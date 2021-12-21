using SixLabors.ImageSharp;


namespace PictCodec.ImageSharp
{
    public class PictConfigurationModule : IConfigurationModule
    {
        /// <inheritdoc/>
        public void Configure(Configuration configuration)
        {
            configuration.ImageFormatsManager.SetEncoder(PictFormat.Instance, new PictEncoder());
        }
    }
}
