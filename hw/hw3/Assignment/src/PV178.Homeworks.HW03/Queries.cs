using PV178.Homeworks.HW03.DataLoading.DataContext;
using PV178.Homeworks.HW03.DataLoading.Factory;
using PV178.Homeworks.HW03.Model;
using PV178.Homeworks.HW03.Model.Enums;


namespace PV178.Homeworks.HW03
{
    public class Queries
    {
        private IDataContext? _dataContext;
        public IDataContext DataContext => _dataContext ??= new DataContextFactory().CreateDataContext();

        /// <summary>
        /// Ukážkové query na pripomenutie základnej LINQ syntaxe a operátorov. Výsledky nie je nutné vracať
        /// pomocou jedného príkazu, pri zložitejších queries je vhodné si vytvoriť pomocné premenné cez `var`.
        /// Toto query nie je súčasťou hodnotenia.
        /// </summary>
        /// <returns>The query result</returns>
        public int SampleQuery()
        {
            return DataContext.Countries
                .Where(a => a.Name?[0] >= 'A' && a.Name?[0] <= 'G')
                .Join(DataContext.SharkAttacks,
                    country => country.Id,
                    attack => attack.CountryId,
                    (country, attack) => new { country, attack }
                )
                .Join(DataContext.AttackedPeople,
                    ca => ca.attack.AttackedPersonId,
                    person => person.Id,
                    (ca, person) => new { ca, person }
                )
                .Where(p => p.person.Sex == Sex.Male)
                .Count(a => a.person.Age >= 15 && a.person.Age <= 40);
        }

        /// <summary>
        /// Úloha č. 1
        ///
        /// Vráťte zoznam, v ktorom je textová informácia o každom človeku,
        /// na ktorého v štáte Bahamas zaútočil žralok s latinským názvom začínajúcim sa 
        /// na písmeno I alebo N.
        /// 
        /// Túto informáciu uveďte v tvare:
        /// "{meno človeka} was attacked in Bahamas by {latinský názov žraloka}"
        /// </summary>
        /// <returns>The query result</returns>
        public List<string> InfoAboutPeopleThatNamesStartsWithCAndWasInBahamasQuery()
        {
            //find bahamas and return empty list if they don't exist
            var bahamas = DataContext.Countries.FirstOrDefault(c => c.Name == "Bahamas");
            if (bahamas == null)
                return new List<string>();

            //get species starting with I or N
            var INSpecies = DataContext.SharkSpecies
                .Where(s => 
                    s.LatinName != null &&
                    (s.LatinName.StartsWith('I') || s.LatinName.StartsWith('N'))
                );

            //get shark attacks in bahamas, get their latin name, get attacked people
            //create people and latin name strings
            return DataContext.SharkAttacks
                .Where(c => c.CountryId == bahamas.Id)
                .Join(INSpecies,
                    attack => attack.SharkSpeciesId,
                    species => species.Id,
                    (attack, species) => new { attack, SpeciesName = species.LatinName }
                )
                .Join(DataContext.AttackedPeople,
                    attName => attName.attack.AttackedPersonId,
                    person => person.Id,
                    (attName, person) => new { attName.SpeciesName, person.Name }
                )
                .Select(sn => $"{sn.Name} was attacked in Bahamas by {sn.SpeciesName}")
                .ToList();
        }

        /// <summary>
        /// Úloha č. 2
        ///
        /// Firma by chcela expandovať do krajín s nedemokratickou formou vlády – monarchie alebo teritória.
        /// Pre účely propagačnej kampane by chcela ukázať, že žraloky v týchto krajinách na ľudí neútočia
        /// s úmyslom zabiť, ale chcú sa s nimi iba hrať.
        /// 
        /// Vráťte súhrnný počet žraločích utokov, pri ktorých nebolo preukázané, že skončili fatálne.
        /// 
        /// Požadovany súčet vykonajte iba pre krajiny s vládnou formou typu 'Monarchy' alebo 'Territory'.
        /// </summary>
        /// <returns>The query result</returns>
        public int FortunateSharkAttacksSumWithinMonarchyOrTerritoryQuery()
        {
            //get valid governments, get attacks, count non-fatal attacks
            return DataContext.Countries
                .Where(c => c.GovernmentForm == GovernmentForm.Monarchy || c.GovernmentForm == GovernmentForm.Territory)
                .Join(DataContext.SharkAttacks,
                    country => country.Id,
                    attack => attack.CountryId,
                    (country, attack) => attack
                )
                .Count(a => a.AttackSeverenity != AttackSeverenity.Fatal);
        }

        /// <summary>
        /// Úloha č. 3
        ///
        /// Marketingovému oddeleniu dochádzajú nápady ako pomenovávať nové produkty.
        /// 
        /// Inšpirovať sa chcú prezývkami žralokov, ktorí majú na svedomí najviac
        /// útokov v krajinách na kontinente 'South America'. Pre pochopenie potrebujú 
        /// tieto informácie vo formáte slovníku:
        /// 
        /// (názov krajiny) -> (prezývka žraloka s najviac útokmi v danej krajine)
        /// </summary>
        /// <returns>The query result</returns>
        public Dictionary<string, string> MostProlificNicknamesInCountriesQuery()
        {
            //get attacks where species has a nickname
            var attacksWithNicknames = DataContext.SharkSpecies
                .Where(s => s.AlsoKnownAs != null && s.AlsoKnownAs != "")
                .Join(DataContext.SharkAttacks,
                    species => species.Id,
                    attack => attack.SharkSpeciesId,
                    (species, attack) => new { attack, Nickname = species.AlsoKnownAs! }    //AlsoKnowAs cannot be null here
                );

            //get south american continents, group countries, nicknames and attacks and extract nicknames
            //group it by country and species name combinations, order by number of items under each group
            //pick the largest groups and finally create the dictionary
            return DataContext.Countries
                .Where(c => c.Continent == "South America" && c.Name != null)
                .GroupJoin(attacksWithNicknames,
                    country => country.Id,
                    attackNick => attackNick.attack.CountryId,
                    (country, attackNick) => new { CountryName = country.Name!, Nicknames = attackNick }    //country name cannot be null here
                )
                .SelectMany(g => g.Nicknames, (g, Nicknames) => new { g.CountryName, Nicknames.Nickname })
                .GroupBy(g => new { g.CountryName, g.Nickname })
                .OrderByDescending(g => g.Count())
                .DistinctBy(g => g.Key.CountryName)
                .ToDictionary(g => g.Key.CountryName, g => g.Key.Nickname);
        }

        /// <summary>
        /// Úloha č. 4
        ///
        /// Firma chce začať kompenzačnú kampaň a potrebuje k tomu dáta.
        ///
        /// Preto zistite, ktoré žraloky útočia najviac na mužov. 
        /// Vráťte ID prvých troch žralokov, zoradených zostupne podľa počtu útokov na mužoch.
        /// </summary>
        /// <returns>The query result</returns>
        public List<int> ThreeSharksOrderedByNumberOfAttacksOnMenQuery()
        {
            //get attacks where males were attacked, group it by species
            //order it by group size, return list of ids
            return DataContext.AttackedPeople
                .Where(p => p.Sex == Sex.Male)
                .Join(DataContext.SharkAttacks,
                    person => person.Id,
                    attack => attack.AttackedPersonId,
                    (person, attack) => attack
                )
                .GroupBy(a => a.SharkSpeciesId)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .Take(3)
                .ToList();
        }

        /// <summary>
        /// Úloha č. 5
        ///
        /// Oslovila nás medzinárodná plavecká organizácia. Chce svojich plavcov motivovať možnosťou
        /// úteku pred útokom žraloka.
        ///
        /// Potrebuje preto informácie o priemernej rýchlosti žralokov, ktorí
        /// útočili na plávajúcich ľudí (informácie o aktivite počas útoku obsahuje "Swimming" alebo "swimming").
        /// 
        /// Pozor, dáta požadajú oddeliť podľa jednotlivých kontinentov. Ignorujte útoky takých druhov žralokov,
        /// u ktorých nie je známa maximálná rýchlosť. Priemerné rýchlosti budú zaokrúhlené na dve desatinné miesta.
        /// </summary>
        /// <returns>The query result</returns>
        public Dictionary<string, double> SwimmerAttacksSharkAverageSpeedQuery()
        {
            //get attacks where the activity contained swimming
            var swimmingAttacks = DataContext.SharkAttacks
                .Where(a => a.Activity != null && a.Activity.ToLower().Contains("swimming"));

            //get all species with valid speed
            var speciesWithSpeed = DataContext.SharkSpecies
                .Where(s => s.TopSpeed.HasValue);

            //group continents and shark species, remove any empty groups, group it by continent
            //average top speed of all species for the continent (using ToDouble(string) for 2 decimal places)
            return DataContext.Countries
                .Join(swimmingAttacks,
                    country => country.Id,
                    attack => attack.CountryId,
                    (country, attack) => new { country.Continent, attack.SharkSpeciesId }
                )
                .GroupJoin(speciesWithSpeed,
                    countryAttack => countryAttack.SharkSpeciesId,
                    species => species.Id,
                    (countryAttack, species) => new { countryAttack.Continent, Species = species }
                )
                .Where(g => g.Species.Any() && g.Continent != null)
                .GroupBy(g => g.Continent)
                .ToDictionary(
                    g => g.Key!,    //Key cannot be null here
                    g => Convert.ToDouble(
                        g.SelectMany(cs => cs.Species.Where(s => s.TopSpeed.HasValue))
                        .Average(s => s.TopSpeed!.Value)    //topspeed cannot be null here
                        .ToString("0.00")
                    )
                );
        }

        /// <summary>
        /// Úloha č. 6
        ///
        /// Zistite všetky nefatálne (AttackSeverenity.NonFatal) útoky spôsobené pri člnkovaní 
        /// (AttackType.Boating), ktoré mal na vine žralok s prezývkou "Zambesi shark".
        /// Do výsledku počítajte iba útoky z obdobia po 3. 3. 1960 (vrátane) a ľudí,
        /// ktorých meno začína na písmeno z intervalu <D, K> (tiež vrátane).
        /// 
        /// Výsledný zoznam mien zoraďte abecedne.
        /// </summary>
        /// <returns>The query result</returns>
        public List<string> NonFatalAttemptOfZambeziSharkOnPeopleBetweenDAndKQuery()
        {
            //get zambesi shark
            var zambesiShark = DataContext.SharkSpecies
                .FirstOrDefault(s => s.AlsoKnownAs == "Zambesi shark");
            if (zambesiShark == null)
                return new List<string>();

            //filter required attacks, get attacked people, select valid names, reutnr them
            return DataContext.SharkAttacks
                .Where(a =>
                    a.AttackSeverenity == AttackSeverenity.NonFatal &&
                    a.Type == AttackType.Boating &&
                    a.SharkSpeciesId == zambesiShark!.Id && //id cannot be null here
                    a.DateTime >= new DateTime(1960, 3, 3)  //this probably creates DateTime every time
                )
                .Join(DataContext.AttackedPeople,
                    attack => attack.AttackedPersonId,
                    person => person.Id,
                    (attack, person) => person
                )
                .Where(p => p.Name != null && p.Name[0] >= 'D' && p.Name[0] <= 'K')
                .Select(p => p.Name!)   //name cannot be null here
                .ToList();
        }

        /// <summary>
        /// Úloha č. 7
        ///
        /// Zistilo sa, že desať najľahších žralokov sa správalo veľmi podozrivo počas útokov v štátoch Južnej Ameriky.
        /// 
        /// Vráťte preto zoznam dvojíc, kde pre každý štát z Južnej Ameriky bude uvedený zoznam žralokov,
        /// ktorí v tom štáte útočili. V tomto zozname môžu figurovať len vyššie spomínaných desať najľahších žralokov.
        /// 
        /// Pokiaľ v nejakom štáte neútočil žiaden z najľahších žralokov, zoznam žralokov u takého štátu bude prázdny.
        /// </summary>
        /// <returns>The query result</returns>
        public List<Tuple<string, List<SharkSpecies>>> LightestSharksInSouthAmericaQuery()
        {
            //get attacks of the 10 lightest species
            var lightSharkAttacks = DataContext.SharkSpecies
                .OrderBy(s => s.Weight)
                .Take(10)
                .Join(DataContext.SharkAttacks,
                    species => species.Id,
                    attack => attack.SharkSpeciesId,
                    (species, attack) => new { Species = species, Attack = attack }
                );

            //filter south american countries, get list of distinct species for every country
            return DataContext.Countries
                .Where(c => c.Continent == "South America" && c.Name != null)
                .GroupJoin(lightSharkAttacks,
                    countries => countries.Id,
                    lightAttack => lightAttack.Attack.CountryId,
                    (countries, lightAttack) => new { 
                        Countries = countries,
                        Species = lightAttack
                            .DistinctBy(sa => sa.Species.Id)
                            .Select(sa => sa.Species)
                    }
                )
                .GroupBy(g => g.Countries.Name!)    //name cannot be null here
                .Select(g => Tuple.Create(g.Key, g.SelectMany(cs => cs.Species).ToList()))
                .ToList();
        }

        /// <summary>
        /// Úloha č. 8
        ///
        /// Napísať hocijaký LINQ dotaz musí byť pre Vás už triviálne. Riaditeľ firmy vás preto chce
        /// využiť na testovanie svojich šialených hypotéz.
        /// 
        /// Zistite, či každý žralok, ktorý má maximálnu rýchlosť aspoň 56 km/h zaútočil aspoň raz na
        /// človeka, ktorý mal viac ako 56 rokov. Výsledok reprezentujte ako pravdivostnú hodnotu.
        /// </summary>
        /// <returns>The query result</returns>
        public bool FiftySixMaxSpeedAndAgeQuery()
        {
            //get all fast sharks and their attacks, group it with all attackd people
            //group it by species, check that all species attacked person with age > 56
            return DataContext.SharkSpecies
                .Where(s => s.TopSpeed >= 56)
                .Join(DataContext.SharkAttacks,
                    species => species.Id,
                    attack => attack.SharkSpeciesId,
                    (species, attack) => new { Species = species, Attacks = attack }
                )
                .GroupJoin(DataContext.AttackedPeople,
                    specAtt => specAtt.Attacks.AttackedPersonId,
                    person => person.Id,
                    (specAtt, people) => new { SpeciesId = specAtt.Species.Id, People = people }
                )
                .GroupBy(g => g.SpeciesId)
                .Select(g => new {
                    SpciesId = g.Key,
                    HasPeople = g.Any(sp => sp.People.Any(p => p.Age > 56))
                })
                .All(sp => sp.HasPeople == true);
        }

        /// <summary>
        /// Úloha č. 9
        ///
        /// Ohromili ste svojim výkonom natoľko, že si od Vás objednali rovno textové výpisy.
        /// Samozrejme, že sa to dá zvladnúť pomocou LINQ.
        /// 
        /// Chcú, aby ste pre všetky fatálne útoky v štátoch začínajúcich na písmeno 'B' alebo 'R' urobili výpis v podobe: 
        /// "{Meno obete} was attacked in {názov štátu} by {latinský názov žraloka}"
        /// 
        /// Záznam, u ktorého obeť nemá meno
        /// (údaj neexistuje, nejde o vlastné meno začínajúce na veľké písmeno, či údaj začína číslovkou)
        /// do výsledku nezaraďujte. Získané pole zoraďte abecedne a vraťte prvých 5 viet.
        /// </summary>
        /// <returns>The query result</returns>
        public List<string> InfoAboutPeopleAndCountriesOnBorRAndFatalAttacksQuery()
        {
            //gett fatal attacks and shark names
            var fatalAttacksLatName = DataContext.SharkAttacks
                .Where(a => a.AttackSeverenity == AttackSeverenity.Fatal)
                .Join(DataContext.SharkSpecies,
                    attack => attack.SharkSpeciesId,
                    species => species.Id,
                    (attack, species) => new { Attack = attack, SpeciesNames = species.LatinName }
                );

            //get countries starting with B or R
            var BRCountries = DataContext.Countries
                .Where(c => c.Name != null && (c.Name.StartsWith('B') || c.Name.StartsWith('R')));

            //get people will valid name, join countries and species to people using attacks
            //create the wanted string, order it alphabetically and take first 5
            return DataContext.AttackedPeople
                .Where(p => p.Name != null && Char.IsUpper(p.Name[0]))
                .Join(fatalAttacksLatName,
                    person => person.Id,
                    fatalAtt => fatalAtt.Attack.AttackedPersonId,
                    (person, fatalAtt) => new { Person = person, fatalAtt.Attack, SpeciesName = fatalAtt.SpeciesNames }
                )
                .Join(BRCountries,
                    pas => pas.Attack.CountryId,
                    country => country.Id,
                    (pas, country) => new { PersonName = pas.Person.Name, pas.SpeciesName, CountryName = country.Name }
                )
                .Where(psc => psc.PersonName != null && psc.CountryName != null && psc.SpeciesName != null)
                .Select(psc => $"{psc.PersonName} was attacked in {psc.CountryName} by {psc.SpeciesName}")
                .OrderBy(str => str)
                .Take(5)
                .ToList();
        }

        /// <summary>
        /// Úloha č. 10
        ///
        /// Nedávno vyšiel zákon, že každá krajina Európy začínajúca na písmeno A až L (vrátane)
        /// musí zaplatiť pokutu 250 jednotiek svojej meny za každý žraločí útok na ich území.
        /// Pokiaľ bol tento útok smrteľný, musia dokonca zaplatiť 300 jednotiek. Ak sa nezachovali
        /// údaje o tom, či bol daný útok smrteľný alebo nie, nemusia platiť nič.
        /// Áno, tento zákon je spravodlivý...
        /// 
        /// Vráťte informácie o výške pokuty európskych krajín začínajúcich na A až L (vrátane).
        /// Tieto informácie zoraďte zostupne podľa počtu peňazí, ktoré musia tieto krajiny zaplatiť.
        /// Vo finále vráťte 5 záznamov s najvyššou hodnotou pokuty.
        /// 
        /// V nasledujúcej sekcii môžete vidieť príklad výstupu v prípade, keby na Slovensku boli 2 smrteľné útoky,
        /// v Česku jeden fatálny + jeden nefatálny a v Maďarsku žiadny:
        /// <code>
        /// Slovakia: 600 EUR
        /// Czech Republic: 550 CZK
        /// Hungary: 0 HUF
        /// </code>
        /// 
        /// </summary>
        /// <returns>The query result</returns>
        public List<string> InfoAboutFinesInEuropeQuery()
        {
            //EU countries with name between A and L
            var ALEuCountries = DataContext.Countries
                .Where(c => c.Continent == "Europe" && c.Name != null && c.Name[0] >= 'A' && c.Name[0] <= 'L');

            //get attacks with know severenity, group it by countries, extract country name, currency and sum of the fine
            //order it by size of the fine, create the string and take firt 5
            return DataContext.SharkAttacks
            .Where(a => a.AttackSeverenity.HasValue && a.AttackSeverenity != AttackSeverenity.Unknown)
            .Join(ALEuCountries,
                attack => attack.CountryId,
                country => country.Id,
                (attack, country) => new {
                    Country = country,
                    Fine = attack.AttackSeverenity == AttackSeverenity.Fatal ? 300 : 250    //do we count null attacks?
                }
            )
            .GroupBy(cf => cf.Country)
            .Select(g => new {
                CountryName = g.Key.Name,
                g.Key.CurrencyCode,
                Fine = g.Sum(cf => cf.Fine)
            })
            .Where(ccf => ccf.CountryName != null && ccf.CurrencyCode != null)
            .OrderByDescending(ccf => ccf.Fine)
            .Select (ccf => $"{ccf.CountryName}: {ccf.Fine} {ccf.CurrencyCode}")
            .Take(5)
            .ToList();
        }

        /// <summary>
        /// Úloha č. 11
        ///
        /// Organizácia spojených žraločích národov výhlásila súťaž: 5 druhov žralokov, 
        /// ktoré sú najviac agresívne získa hodnotné ceny.
        /// 
        /// Nájdite 5 žraločích druhov, ktoré majú na svedomí najviac ľudských životov,
        /// druhy zoraďte podľa počtu obetí.
        ///
        /// Výsledok vráťte vo forme slovníku, kde
        /// kľúčom je meno žraločieho druhu a
        /// hodnotou je súhrnný počet obetí spôsobený daným druhom žraloka.
        /// </summary>
        /// <returns>The query result</returns>
        public Dictionary<string, int> FiveSharkNamesWithMostFatalitiesQuery()
        {
            //get fatal attacks, get species names, group it by species name
            //order it by number of attacks per species
            return DataContext.SharkAttacks
                .Where(a => a.AttackSeverenity == AttackSeverenity.Fatal)
                .Join(DataContext.SharkSpecies,
                    attack => attack.SharkSpeciesId,
                    species => species.Id,
                    (attack, species) => new { Attack = attack, SpeciesName = species.Name }
                )
                .Where(an => an.SpeciesName != null)
                .GroupBy(an => an.SpeciesName)
                .Select(g => new {SpeciesName = g.Key, AttackCount = g.Count()})
                .OrderByDescending(sa => sa.AttackCount)
                .Take(5)
                .ToDictionary(sa => sa.SpeciesName!, sa => sa.AttackCount); //speciesname cannot be null here
        }

        /// <summary>
        /// Úloha č. 12
        ///
        /// Riaditeľ firmy chce si chce podmaňiť čo najviac krajín na svete. Chce preto zistiť,
        /// na aký druh vlády sa má zamerať, aby prebral čo najviac krajín.
        /// 
        /// Preto od Vás chce, aby ste mu pomohli zistiť, aké percentuálne zastúpenie majú jednotlivé typy vlád.
        /// Požaduje to ako jeden string:
        /// "{1. typ vlády}: {jej percentuálne zastúpenie}%, {2. typ vlády}: {jej percentuálne zastúpenie}%, ...".
        /// 
        /// Výstup je potrebný mať zoradený od najväčších percent po najmenšie,
        /// pričom percentá riaditeľ vyžaduje zaokrúhľovať na jedno desatinné miesto.
        /// Pre zlúčenie musíte podľa jeho slov použiť metódu `Aggregate`.
        /// </summary>
        /// <returns>The query result</returns>
        public string StatisticsAboutGovernmentsQuery()
        {
            //group countries by government type, get representation
            //order it by representation, create required string
            return DataContext.Countries
                .GroupBy(c => c.GovernmentForm)
                .Select(g => new {
                    GovernmentForm = g.Key,
                    Representation = ((double)g.Count() / DataContext.Countries.Count) * 100
                })
                .OrderByDescending(gr => gr.Representation)
                .Aggregate("", (acc, gr) => acc += $", {gr.GovernmentForm}: {gr.Representation:F1}%")[2..];
        }

        /// <summary>
        /// Úloha č. 13
        ///
        /// Firma zistila, že výrobky s tigrovaným vzorom sú veľmi populárne. Chce to preto aplikovať
        /// na svoju doménu.
        ///
        /// Nájdite informácie o ľudoch, ktorí boli obeťami útoku žraloka s menom "Tiger shark"
        /// a útok sa odohral v roku 2001.
        /// Výpis majte vo formáte:
        /// "{meno obete} was tiggered in {krajina, kde sa útok odohral}".
        /// V prípade, že chýba informácia o krajine útoku, uveďte namiesto názvu krajiny reťazec "Unknown country".
        /// V prípade, že informácie o obete vôbec neexistuje, útok ignorujte.
        ///
        /// Ale pozor! Váš nový nadriadený má panický strach z operácie `Join` alebo `gJoin`.
        /// Informácie musíte zistiť bez spojenia hocijakých dvoch tabuliek. Skúste sa napríklad zamyslieť,
        /// či by vám pomohla metóda `Zip`.
        /// </summary>
        /// <returns>The query result</returns>
        public List<string> TigerSharkAttackZipQuery()
        {
            //get attacks from year 2001 with know victim
            var Attacks2001 = DataContext.SharkAttacks
                .Where(a => a.DateTime.HasValue && a.DateTime.Value.Year == 2001)
                .Where(a => a.AttackedPersonId.HasValue);

            //get species for every attack, get only tiger shark attacks, create wanted string
            return Attacks2001
                 .Select(a => DataContext.SharkSpecies.FirstOrDefault(s => s.Id == a.SharkSpeciesId))
                 .Zip(Attacks2001, (s, a) => new { Species = s, Attack = a })
                 .Where(sa => sa.Species != null && sa.Species.Name == "Tiger shark")
                 .Select(sa =>
                     $"{DataContext.AttackedPeople.First(p => p.Id == sa.Attack.AttackedPersonId).Name} was tiggered in " +
                     $"{DataContext.Countries.FirstOrDefault(c => c.Id == sa.Attack.CountryId)?.Name ?? "Unknown country"}"
                 )
                 .ToList();
        }

        /// <summary>
        /// Úloha č. 14
        ///
        /// Vedúci oddelenia prišiel s ďalšou zaujímavou hypotézou. Myslí si, že veľkosť žraloka nejako 
        /// súvisí s jeho apetítom na ľudí.
        ///
        /// Zistite pre neho údaj, koľko percent útokov má na svedomí najväčší a koľko najmenší žralok.
        /// Veľkosť v tomto prípade uvažujeme podľa dĺžky.
        /// 
        /// Výstup vytvorte vo formáte: "{percentuálne zastúpenie najväčšieho}% vs {percentuálne zastúpenie najmenšieho}%"
        /// Percentuálne zastúpenie zaokrúhlite na jedno desatinné miesto.
        /// </summary>
        /// <returns>The query result</returns>
        public string LongestVsShortestSharkQuery()
        {
            //group and order all attacks by the species length
            var query = DataContext.SharkAttacks
                .Join(DataContext.SharkSpecies,
                    attack => attack.SharkSpeciesId,
                    species => species.Id,
                    (attack, species) => new { Species = species, Attack = attack }
                )
                .GroupBy(sa => sa.Species.Length)
                .OrderBy(sa => sa.Key);

            //get shart attack count
            var attackCount = DataContext.SharkAttacks.Count;

            //return the final string
            return $"{((double)query.Last().Count() / attackCount) * 100 :F1}% vs " +
                   $"{((double)query.First().Count() / attackCount) * 100:F1}%";
        }

        /// <summary>
        /// Úloha č. 15
        ///
        /// Na koniec vašej kariéry Vám chceme všetci poďakovať a pripomenúť Vám vašu mlčanlivosť.
        /// 
        /// Ako výstup požadujeme počet krajín, v ktorých žralok nespôsobil smrť (útok nebol fatálny).
        /// Berte do úvahy aj tie krajiny, kde žralok vôbec neútočil.
        /// </summary>
        /// <returns>The query result</returns>
        public int SafeCountriesQuery()
        {
            //count countries where fatal attack occured
            var fatalCountryCount = DataContext.SharkAttacks
                .Where(a => a.AttackSeverenity == AttackSeverenity.Fatal)
                .GroupBy(a => a.CountryId)
                .Count(g => g.Key != null);

            //get nonfatal country count
            return DataContext.Countries.Count - fatalCountryCount;
        }
    }
}
