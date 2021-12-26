using SixLabors.ImageSharp;


namespace PictSharp.ImageSharpAdaptor
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
