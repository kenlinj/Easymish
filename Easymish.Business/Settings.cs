
namespace Easymish.Business
{
    public class Settings : ServiceInterface.ISettings
    {
        public string CompanyUrl
        {
            get
            {
                return "http://scimarketview.com/";
            }
        }

        public string CompanyName
        {
            get
            {
                return "SCI MarketView Inc.";
            }
        }
    }
}
