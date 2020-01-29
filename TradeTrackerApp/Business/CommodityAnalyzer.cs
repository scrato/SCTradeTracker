using SCTradeTracker.API;
using SCTradeTracker.ComputerVision;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SCTradeTracker.Business
{
    public class CommodityAnalyzer
    {
        private CVClient ComputerVision { get; } = new CVClient();
        private Screenshot Screenshot { get; } = new Screenshot();
        public async Task AnalyseAsync()
        {
            var bitmap = Screenshot.ActiveWindow();
            var fileName = $"{DateTime.Now.ToString("yyyy_MM_dd_hh_mm_")}{Guid.NewGuid().ToString()}.png";
            bitmap.Save(fileName, ImageFormat.Png);
            await ComputerVision.ReadBitmapAsync(fileName, CancellationToken.None);
        }
    }
}
