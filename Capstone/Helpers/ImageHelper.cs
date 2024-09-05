using Capstone.Models;

namespace Capstone.Helpers
{
    public static class ImageHelper
    {
        public static List<EventImg> HandleEventImg(List<MemoryStream> imageStreams, Event associatedEvent)
        {
            var eventImages = new List<EventImg>();

            foreach (var imageStream in imageStreams)
            {
                if (imageStream != null)
                {
                    imageStream.Position = 0;
                    var imgData = imageStream.ToArray();

                    var eventImg = new EventImg
                    {
                        ImgData = imgData,
                        Event = associatedEvent
                    };

                    eventImages.Add(eventImg);
                }
            }

            return eventImages;
        }

    }
}
