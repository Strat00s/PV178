using PV178.Homeworks.HW03.DataLoading.DataContext;
using PV178.Homeworks.HW03.DataLoading.Factory;
using PV178.Homeworks.HW03.Model;
using PV178.Homeworks.HW03.Model.Enums;
using System.Security.Cryptography.X509Certificates;

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
                .Where(a => a.Name?[0] >= 'A' && a.Name?[0] <= 'G')     //countries with names starting with A to G
                .Join(DataContext.SharkAttacks,                         //join it with shark attacks using keys country.Id and countryid
                    country => country.Id,
                    attack => attack.CountryId,
                    (country, attack) => new { country, attack }
                )
                .Join(DataContext.AttackedPeople,                       //join it with people attacked using personId and person.id
                    ca => ca.attack.AttackedPersonId,
                    person => person.Id,
                    (ca, person) => new { ca, person }
                )
                .Where(p => p.person.Sex == Sex.Male)                   //extract males
                .Count(a => a.person.Age >= 15 && a.person.Age <= 40);  //with age between 15 and 40
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
            return DataContext.SharkAttacks
                .Where(a => a.CountryId == DataContext.Countries.First(b => b.Name == "Bahamas").Id)                    //get shark attacks with bahamas id
                .Join(DataContext.SharkSpecies.Where(s => s.LatinName.StartsWith("I") || s.LatinName.StartsWith("N")),  //join it by speciesid that is fillter for species with latin names starting with I or N
                    shark => shark.SharkSpeciesId,
                    species => species.Id,
                    (shark, species) => new { shark, species }                                                          //save shark and species
                )
                .Join(DataContext.AttackedPeople,                                                                       //join it with attacked people
                    sharkInfo => sharkInfo.shark.AttackedPersonId,
                    person => person.Id,
                    (sharkInfo, person) => new { sharkInfo.species.LatinName, person.Name }                             //save latin name and person name
                )
                .Select(a => $"{a.Name} was attacked in Bahamas by {a.LatinName}")                                      //create the string
                .ToList();                                                                                              //convert it to list
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
            return DataContext.Countries
                .Where(a => a.GovernmentForm == GovernmentForm.Monarchy || a.GovernmentForm == GovernmentForm.Territory)
                .Join(DataContext.SharkAttacks,
                    country => country.Id,
                    shark => shark.CountryId,
                    (country, shark) => new { shark }
                )
                .Count(a => a.shark.AttackSeverenity != AttackSeverenity.Fatal);
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
            var attacksWithNicknames = DataContext.SharkSpecies
                .Where(s => s.AlsoKnownAs != null && s.AlsoKnownAs != "")
                .Join(DataContext.SharkAttacks,
                    species => species.Id,
                    attack => attack.SharkSpeciesId,
                    (species, attack) => new { attack, nickName = species.AlsoKnownAs! }
                );

            return DataContext.Countries
                .Where(a => a.Continent == "South America")                                                     //get south america continents
                .GroupJoin(attacksWithNicknames,                                                                //get nicknames for the attack at the continents
                    country => country.Id,
                    attackNick => attackNick.attack.CountryId,
                    (country, attackNick) => new { countryName = country.Name!, attackNick }
                )
                .SelectMany(e => e.attackNick, (e, attackNick) => new { e.countryName, attackNick.nickName })   //flatten the attackNick to keep only nicknames
                .GroupBy(p => new { p.countryName, p.nickName })                                                //g everything
                .OrderByDescending(g => g.Count())                                                              //order it by g size
                .DistinctBy(g => g.Key.countryName)                                                             //remove everything but first occurance of each continent (leaving only biggest g)
                .ToDictionary(g => g.Key.countryName, g => g.Key.nickName);                                     //make dictionary
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
            return DataContext.AttackedPeople
                .Where(p => p.Sex == Sex.Male)          //males only
                .Join(DataContext.SharkAttacks,         //get attack that occured on males
                    person => person.Id,
                    attack => attack.AttackedPersonId,
                    (person, attack) => attack
                )
                .GroupBy(a => a.SharkSpeciesId)         //group it by sharkspecies
                .OrderByDescending(g => g.Count())      //get the most occuring species id
                .Select(g => g.Key)                     //select only keys (species id)
                .Take(3)                                //we want first 3
                .ToList();                              //make it a list
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
        //TODO refactor
        public Dictionary<string, double> SwimmerAttacksSharkAverageSpeedQuery()
        {
            //get attacks where the activity contained swimming
            var swimmingAttacks = DataContext.SharkAttacks
                .Where(a => a.Activity!.Contains("Swimming") || a.Activity!.Contains("swimming"));

            //get all species with valid speed
            var speciesWithSpeed = DataContext.SharkSpecies
                .Where(s => s.TopSpeed.HasValue);

            return DataContext.Countries
                .Join(swimmingAttacks,                                                                      //get countries with swimming attacks
                    country => country.Id,
                    attack => attack.CountryId,
                    (country, attack) => new { Country = country, Attack = attack }
                )
                .GroupJoin(speciesWithSpeed,                                                                //get species for every attack in every country
                    data => data.Attack.SharkSpeciesId,
                    species => species.Id,
                    (data, species) => new { Continent = data.Country.Continent, Species = species }
                )
                .Where(g => g.Species.Any())                                                                //remove empty species
                .GroupBy(g => g.Continent)                                                                  //group it by continent
                .Select(g => new                                                                            //get desired data
                {
                    Continent = g.Key!,
                    Speed = g.SelectMany(x => x.Species).Average(s => s?.TopSpeed ?? 0).ToString("0.00")    //TopSpeed will always have a value, but ToString is complaining about it being nullable
                })
                .ToDictionary(                                                                              //convert it to the resulting dictionary
                    g => g.Continent,
                    g => Convert.ToDouble(g.Speed)
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
            var zambesiSharkId = DataContext.SharkSpecies.First(s => s.AlsoKnownAs == "Zambesi shark").Id;

            return DataContext.SharkAttacks
                .Where(a =>
                    a.AttackSeverenity == AttackSeverenity.NonFatal &&              //only nonfatal attack
                    a.Type == AttackType.Boating &&                                 //only boating type
                    a.SharkSpeciesId == zambesiSharkId &&                           //only zambesi shark
                    a.DateTime >= new DateTime(1960, 3, 3)                          //after 3.3.1960
                )
                .Join(DataContext.AttackedPeople,                                   //join attacks with people and keep only people
                    attacks => attacks.AttackedPersonId,
                    people => people.Id,
                    (attacks, people) => people
                )
                .Where(p => p.Name != null && p.Name[0] >= 'D' && p.Name[0] <= 'K') //check names
                .Select(p => p.Name!)                                               //get only names
                .ToList();                                                          //return them as list
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
            var lightSharkAttacks = DataContext.SharkSpecies
                .OrderBy(s => s.Weight)
                .Take(10)
                .Join(DataContext.SharkAttacks,
                    species => species.Id,
                    attacks => attacks.SharkSpeciesId,
                    (species, attacks) => new { Attacks = attacks, Species = species }
                );

            return DataContext.Countries
                .Where(c => c.Continent == "South America")
                .GroupJoin(lightSharkAttacks,
                    countries => countries.Id,
                    data => data.Attacks.CountryId,
                    (countries, data) => new { 
                        Countries = countries,
                        Species = data
                            .DistinctBy(x => x.Species.Id)
                            .Select(x => x.Species)
                    }
                )
                .GroupBy(g => g.Countries.Name!)
                .Select(g => Tuple.Create(g.Key, g.SelectMany(x => x.Species).ToList()))
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
            var peopleAbove56 = DataContext.AttackedPeople
                .Where(x => x.Age > 56);

            var speedySpecies = DataContext.SharkSpecies
                .Where(x => x.TopSpeed >= 56);

            var query = DataContext.SharkAttacks
                .Join(DataContext.AttackedPeople.Where(x => x.Age > 56),
                    attacks => attacks.AttackedPersonId,
                    people => people.Id,
                    (attacks, people) => new { Attacks = attacks, People = people }
                )
                .Join(DataContext.SharkSpecies,
                    data => data.Attacks.SharkSpeciesId,
                    species => species.Id,
                    (data, species) => new { data, Species = species }
                )
                .Where(x => x.Species.TopSpeed < 56);

            return !query.Any();
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
            // TODO...
            throw new NotImplementedException();
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
            // TODO...
            throw new NotImplementedException();
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
            // TODO...
            throw new NotImplementedException();
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
            // TODO...
            throw new NotImplementedException();
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
            var Attacks2001 = DataContext.SharkAttacks
                .Where(a => a.DateTime.HasValue && a.DateTime.Value.Year == 2001)   //2001 results only
                .Where(a => a.AttackedPersonId.HasValue);                           //remove attacks without know person

            return Attacks2001
                 .Select(a => DataContext.SharkSpecies.FirstOrDefault(s => s.Id == a.SharkSpeciesId))    //"replace" attacks with species with same id as in the attack
                 .Zip(Attacks2001, (s, a) => new { Attack = a, Species = s?.Name })                      //"add back" the attack
                 .Where(a => a.Species == "Tiger shark")                                                 //filter by species
                 .Select(a =>                                                                            //create the string
                     $"{DataContext.AttackedPeople.First(p => p.Id == a.Attack.AttackedPersonId).Name} was tiggered in " +
                     $"{DataContext.Countries.FirstOrDefault(c => c.Id == a.Attack.CountryId)?.Name ?? "Unknown country"}"
                 )
                 .ToList();                                                                              //convert it to list
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
            // TODO...
            throw new NotImplementedException();
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
            // TODO...
            throw new NotImplementedException();
        }
    }
}
