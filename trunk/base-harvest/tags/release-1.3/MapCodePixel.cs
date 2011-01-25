using Landis.RasterIO;

namespace Landis.Harvest
{
    /// <summary>
    /// A pixel with a 16-bit unsigned map code for a management area or a
    /// stand.
    /// </summary>
    public class MapCodePixel
        : SingleBandPixel<ushort>
    {
    }
}
