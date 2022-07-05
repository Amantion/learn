using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Linq;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using System.Threading;

namespace learnSeleniumSteam
{
  
    public class Tests
    {
        private IWebDriver driver;
      
        public const string mainPageUrl =  "https://store.steampowered.com/";
        private readonly By _aboutButton = By.XPath("//*[@class='supernav_container']//a[contains(@href, 'about')]");
        private readonly By _onlinePlayers = By.XPath("//*[@class='online_stats']//*[@class='online_stat_label gamers_online']/..");
        private readonly By _ingamePlayers = By.XPath("//*[@class='online_stats']//*[@class='online_stat_label gamers_in_game']/..");
        private readonly By _storeLinkPos = By.XPath("//*[@class='supernav_container']/*[@data-tooltip-content='.submenu_store']");
        private readonly By _newAndPr = By.Id("noteworthy_tab");
        private readonly By _newAndPrMenu = By.XPath("//*[@class='popup_block_new flyout_tab_flyout responsive_slidedown'][contains(@style,'display: none')]//../*[@id='noteworthy_tab']");
        private readonly By _newAndPrTopsellers = By.XPath("//*[@class='popup_menu_item'][contains(@href, 'topsellers')]");
        private readonly By _selectLunixOsBox = By.XPath("//*[@data-collapse-name='os']//*[@data-value='linux']");
        private readonly By _coopTypeMenu = By.XPath("//*[@data-collapse-name='category3']");
        private readonly By _coopTypeLinux = By.XPath("//*[@data-collapse-name='category3']//*[@data-value='48']");
        private readonly By _gameTagAction = By.XPath("//*[@data-collapse-name='tags']//*[@data-value='19']");
        private readonly By _searchResultQuantity = By.XPath("//*[@class='search_results_count']");
        private readonly By _firstGameName = By.XPath("//*[@id='search_resultsRows']/a[contains(@data-ds-appid, '')]//*[@class='title']");
        private readonly By _firstReleaseDate = By.XPath("//*[@id='search_resultsRows']/a[contains(@data-ds-appid, '')]//*[@class='col search_released responsive_secondrow']");
        private readonly By _firstGamePrice = By.XPath("//*[@id='search_resultsRows']/a[contains(@data-ds-appid, '')]//*[contains(@class, 'col search_price  responsive_secondrow') or contains(@class, 'col search_price discounted responsive_secondrow')]");



        [SetUp]
        public void Setup()
        {
            driver = new OpenQA.Selenium.Chrome.ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

        }

        [Test]
        public void Test1()
        {
            driver.Navigate().GoToUrl(mainPageUrl);
            Assert.AreEqual(mainPageUrl, driver.Url);
            
            var aboutButton = driver.FindElement(_aboutButton);
            aboutButton.Click();

            Assert.AreEqual("https://store.steampowered.com/about/", driver.Url);

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            wait.Until(driver => driver.FindElement(_onlinePlayers));

            int onlinePlayersValue;
            int.TryParse(string.Join("", (driver.FindElement(_onlinePlayers).Text).Where(c => char.IsDigit(c))), out onlinePlayersValue);

            int ingamePlayersValue;
            int.TryParse(string.Join("", (driver.FindElement(_ingamePlayers).Text).Where(c => char.IsDigit(c))), out ingamePlayersValue);

            Assert.IsTrue(onlinePlayersValue > ingamePlayersValue);


            var storeLink = driver.FindElement(_storeLinkPos);
            storeLink.Click();

            Assert.AreEqual(mainPageUrl, driver.Url);


        }

        [Test]
        public void Test2()
        {
            driver.Navigate().GoToUrl(mainPageUrl);
            Assert.AreEqual(mainPageUrl, driver.Url);

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            Actions action = new Actions(driver);

            wait.Until(driver => driver.FindElement(_newAndPr));
            var newadrpr = driver.FindElement(_newAndPr);        
            
            action.MoveToElement(newadrpr);
            action.Perform();
            wait.Until(driver => driver.FindElement(_newAndPrMenu));
            driver.FindElement(_newAndPrTopsellers).Click();  // можно же не создавать лишние переменные, если можно сразу кликать?
            Assert.IsTrue(driver.Url.Contains("filter=topsellers"));

            driver.FindElement(_selectLunixOsBox).Click();
            Assert.IsTrue(driver.Url.Contains("os=linux"));

            wait.Until(driver => driver.FindElement(_coopTypeMenu));
            driver.FindElement(_coopTypeMenu).Click();
            wait.Until(driver => driver.FindElement(_coopTypeLinux));
            driver.FindElement(_coopTypeLinux).Click();
            Assert.IsTrue(driver.Url.Contains("category3=48"));

            wait.Until(driver => driver.FindElement(_gameTagAction));
            driver.FindElement(_gameTagAction).Click();
            Assert.IsTrue(driver.Url.Contains("tags=19"));

            Thread.Sleep(2000); 

            wait.Until(driver => driver.FindElement(_searchResultQuantity));
            int searchResultValue;
            int.TryParse(string.Join("", (driver.FindElement(_searchResultQuantity).Text).Where(c => char.IsDigit(c))), out searchResultValue);
            driver.FindElement(By.Id("footer_logo_steam")).Click();
            driver.FindElement(By.Id("footer_logo_steam")).Click();

            Thread.Sleep(2000);

            int searchResultArrayValue = driver.FindElements(By.XPath("//*[@id='search_resultsRows']/a[contains(@data-ds-appid, '')]")).Count;
            Assert.AreEqual(searchResultArrayValue, searchResultValue);
            // TestContext.Out.WriteLine(searchResultValue); - оставил для себя, чтоб не забыть

            var firstGameName = driver.FindElement(_firstGameName).Text;
            var firstGameReleaseDate = driver.FindElement(_firstReleaseDate).Text;
            var firstGamePrice = driver.FindElement(_firstGamePrice).Text;
            int firstGameFinalPrice;
            int.TryParse(string.Join("", (driver.FindElement(_firstGamePrice).Text).Where(c => char.IsDigit(c))), out firstGameFinalPrice);

            driver.FindElement(_firstGameName).Click();
            Assert.IsTrue(driver.Url.Contains("/app/"));

            var currentGameName = driver.FindElement(By.Id("appHubAppName")).Text;
            Assert.AreEqual(firstGameName, currentGameName);
            var currentGameReleaseDate = driver.FindElement(By.XPath("//*[@class='date']")).Text;
            Assert.AreEqual(firstGameReleaseDate, currentGameReleaseDate);
            var currentGamePrice = driver.FindElement(By.XPath("//*[@class='discount_prices']")).Text;
            int currentGameFinalPrice;
            int.TryParse(string.Join("", currentGamePrice.Where(c => char.IsDigit(c))), out currentGameFinalPrice);
            Assert.AreEqual(firstGameFinalPrice, currentGameFinalPrice);


        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }
    }
}


