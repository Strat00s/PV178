using NUnit.Framework;
//using TestingClass_03;
using TestingFizzbuzz;

namespace UnitTestsDemo
{
    /*
      * Debugging vsech testu lze spustit pres horni zalozku Test -> Debug -> All tests
      *
      * Zde si představíme Unit Testy.
      * Používáme je na automatické otestování funkcionality dílčích částí kódu(obvykle bereme že metod).
      * Třídy a metody testů musí mít odpovídající hlavičku:
          * Pro MSTest - [TestClass] a [TestMethod] - POZOR vyžaduje pro svůj běh Visual Studio
          * Pro xUnit - <nic> a [Fact]
          * Pro nUnit - [TestFixture] a [Test]
      * Jméno unit testu se podle konvencí skládá ze tří částí:
      * UnitOfWork(testovaná jednotka)_StateUnderTest(stav v kterém jednotku testujeme)_ExpectedBehavior(očekávané chování)
      * Unit test se skládá ze tří částí: Arrange(inicializace prostředí pro test), Act(vykonání metody), Assert(verifikace).
      * Test je typu void (nic nevrací) a nebere žádné argumenty.
      */
    /*[TestFixture]
    public class FactorialTest
    {
        //Nejběžnější test. Prosté porovnání výstupů.
        [Test]
        public void Factorial_OnGeneralInput_Correct()
        {
            //Arrange
            int input = 5;
            int expectedOutput = 120;
            //Act
            int output = Factorial.FactorialGood(input);
            //Assert
            Assert.AreEqual(expectedOutput, output);
        }

        //Test na vyhození vyjímky.
        // Selže pokud nedojde k vyhození výjimky.
        [Test]
        public void Factorial_OnNegativeInput_Throws()
        {
            //Arrange
            int input = -1;
            //Act
            int output = Factorial.FactorialBadExceptionless(input);
            //Assert
            Assert.Throws<ArgumentException>(() => { });
        }

        //Tento test navíc kromě správnosti výstupu kontroluje i dobu výpočtu. Doba je v milisekundách.
        [Test]
        [Timeout(100)]
        public void Factorial_OnGeneralInput_CompletesAndCorrect()
        {
            //Arrange
            int input = 5;
            int expectedOutput = 120;
            //Act
            int output = Factorial.FactorialBadInfinite(input);
            //Assert
            Assert.AreEqual(expectedOutput, output);
        }
    }*/
    [TestFixture]
    public class FizzbuzzTest {
        [Test]
        public void DivThreeTest()
        {
            //Arrange
            int input = 6;
            string expectedOutput = "Fizz";
            //Act
            string output = TestingFizzbuzz.Fizzbuzz.input(input);
            //Assert
            Assert.AreEqual(expectedOutput, output);
        }

        [Test]
        public void DivFiveTest() {
            int input = 5;
            string expectedOutput = "Buzz";
            string output = TestingFizzbuzz.Fizzbuzz.input(input);
            Assert.AreEqual(expectedOutput, output);
        }

        [Test]
        public void DivBoth() {
            int input = 15;
            string expectedOutput = "FizzBuzz";
            string output = TestingFizzbuzz.Fizzbuzz.input(input);
            Assert.AreEqual(expectedOutput, output);
        }

        [Test]
        public void OtherTest() {
            int[] inputs = new int[] {1, 2, 4, 7, 8, 11, 13, 14};
            foreach (var input in inputs) {
                string output = TestingFizzbuzz.Fizzbuzz.input(input);
                Assert.AreEqual(input.ToString(), output);
            }
        }
    }
}
