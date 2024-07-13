using PuppeteerSharp;

namespace Evaluation_3.Models.Export
{
    public class PuppeteerService
    {
        private readonly string _browserExecutablePath;
        private readonly ILogger<PuppeteerService> _logger;


        public PuppeteerService(IWebHostEnvironment env, ILogger<PuppeteerService> logger)
        {
            _logger = logger;
            _browserExecutablePath = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe";

            if (!File.Exists(_browserExecutablePath))
            {
                _logger.LogError("Edge executable not found at: " + _browserExecutablePath);
                throw new FileNotFoundException("Edge executable not found", _browserExecutablePath);
            }

            _logger.LogInformation("Edge executable found at: " + _browserExecutablePath);
        }

        public async Task<Browser> GetBrowserAsync()
        {
            try
            {
                _logger.LogInformation("Launching browser.");
                return (Browser)await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = true,
                    ExecutablePath = _browserExecutablePath
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while launching the browser.");
                throw;
            }
        }
    }
}
