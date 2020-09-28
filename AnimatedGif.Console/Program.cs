using System.Drawing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;
using SixLabors.ImageSharp.PixelFormats;


namespace AnimatedGif.Console
{
    // https://stackoverflow.com/questions/64023538/is-it-possible-to-merge-a-image1-gift-and-image2-jpg-with-net-or-javascript
    // https://www.evoketechnologies.com/blog/code-review-checklist-perform-effective-code-reviews/
    // https://www.michaelagreiler.com/code-review-checklist-2/
    // https://de.wikipedia.org/wiki/Zahlennamen
    // https://en.wikipedia.org/wiki/Names_of_large_numbers

    // https://www.google.com/search?hl=en&output=search&sclient=psy-ab&q=d3js%20tutorial&cad=h
    // https://github.com/d3/d3/wiki/Tutorials
    // https://bost.ocks.org/mike/circles/
    // https://bost.ocks.org/mike/selection/
    public class Program
    {
        public static void ResizeCrop(int width, int height, int x, int y, int cropWidth, int cropHeight)
        {
            string path = "";
            using (System.IO.FileStream inStream = System.IO.File.OpenRead(path))
            {
                using (var outStream = new System.IO.MemoryStream())
                using (var image = Image.Load(inStream, out SixLabors.ImageSharp.Formats.IImageFormat format))
                {
                    var clone = image.Clone(
                        i => i.Resize(width, height)
                            .Crop(new SixLabors.ImageSharp.Rectangle(x, y, cropWidth, cropHeight)));

                    clone.Save(outStream, format);
                }
            } // End Using inStream 
        }


        public static void WriteGif()
        {
            // topLeft 290/45
            // topRight 440/45
            // bottomLeft 290/180
            // bottomRight 440/180

            // x = 290
            // y = 45
            // w = 440 - 290 = 150
            // h = 180 -  45 = 135

            Image<Rgba32> gif = new Image<Rgba32>(480, 480);

            for (int i = 0; i < 70; ++i)
            {
                string fn = "frame-" + i.ToString().PadLeft(2, '0') + ".png";
                using (Image frameImage = Image.Load(fn))
                {
                    // https://stackoverflow.com/questions/50860392/how-to-combine-two-images
                    // GraphicsOptions go = new GraphicsOptions();
                    
                    frameImage.Mutate(imageProcess => imageProcess.DrawImage(gif, PixelColorBlendingMode.Add));
                    
                    
                    // gif.Frames.InsertFrame(0, image.Frames[0]);
                    gif.Frames.AddFrame(frameImage.Frames[0]);
                    gif.Frames[i + 1].Metadata.GetGifMetadata().FrameDelay = 6;
                } // End Using frameImage 
            } // Next i 

            gif.Frames.RemoveFrame(0);

            string path = @"output.gif";
            using (System.IO.FileStream fs = System.IO.File.Create(path))
            {
                // gif.SaveAsGif(fs);
                SixLabors.ImageSharp.Formats.IImageEncoder gifEnc = new SixLabors.ImageSharp.Formats.Gif.GifEncoder();
                gif.Save(fs, gifEnc);
            } // End Using fs
        } // End Sub WriteGif


        public static void ReadGif()
        {
            string path = @"sample.gif";


            // https://stackoverflow.com/questions/56745785/imagesharp-trying-to-make-an-animated-gif-that-count-from-0-to-9
            // https://github.com/SixLabors/ImageSharp/issues/942
            using (Image image = Image.Load(path))
            {
                GifMetadata imageMetadata = image.Metadata.GetFormatMetadata(GifFormat.Instance);
                // SixLabors.ImageSharp.Formats.Gif.GifMetadata gifMeta = image.Metadata.GetGifMetadata();

                for (int i = 0; i < image.Frames.Count; ++i)
                {
                    //s image.Frames.ExportFrame()

                    using (var frameImage = image.Frames.CloneFrame(i))
                    {
                        string outPath = "frame-" + i.ToString().PadLeft(2, '0') + ".png";
                        frameImage.Save(outPath, new PngEncoder()); // we include all metadata from the original image;
                    }

                    ImageFrameMetadata frameMetaData = image.Frames[i].Metadata;
                    GifFrameMetadata frameGifMetaData = frameMetaData.GetFormatMetadata(GifFormat.Instance);
                    System.Console.WriteLine(frameGifMetaData.FrameDelay);
                }


                // image.Frames.AddFrame()
                // GifFrameMetaData frameMetaData = img.MetaData.GetFormatMetaData(GifFormat.Instance);
                // frameMetaData.FrameDelay = 100;// 1 second


                // Resize the image in place and return it for chaining.
                // 'x' signifies the current image processing context.
                // image.Mutate(x => x.Resize(image.Width / 2, image.Height / 2)); 

                // The library automatically picks an encoder based on the file extension then
                // encodes and write the data to disk.
                // You can optionally set the encoder to choose.
                // image.Save("bar.jpg"); 
            } // Dispose - releasing memory into a memory pool ready for the next image you wish to process.
        }


        public static void GifInfoTest()
        {
            string path = @"sample.gif";

            GifInfo info = new GifInfo(path);
            System.Console.WriteLine(info);


            for (int i = 0; i < info.Frames.Count - 1; i++)
            {
                string frameFilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(path),
                    $"{System.IO.Path.GetFileNameWithoutExtension(path)}-{i.ToString().PadLeft(2, '0')}.png");
                System.Drawing.Image frame = info.Frames[i];

                // System.Console.WriteLine(frame);
                System.Console.WriteLine(frameFilePath);
                frame.Save(frameFilePath, System.Drawing.Imaging.ImageFormat.Png);
            } // Next i 
        } // End Sub GifInfoTest 


        public static void GifCreatorTest()
        {
            // 33ms delay (~30fps)
            using (AnimatedGifCreator gif = AnimatedGif.Create("gif.gif", 33))
            {
                System.Drawing.Image img1 = System.Drawing.Image.FromFile("img1.png");
                gif.AddFrame(img1, delay: 2000, quality: GifQuality.Bit8);
                System.Drawing.Image img2 = System.Drawing.Image.FromFile("img2.png");
                gif.AddFrame(img2, delay: 2000, quality: GifQuality.Bit8);
                System.Drawing.Image img3 = System.Drawing.Image.FromFile("img3.png");
                gif.AddFrame(img3, delay: 2000, quality: GifQuality.Bit8);
            } // End Using gif 
        } // End Sub GifCreatorTest 


        public static void Main(string[] args)
        {
            WriteGif();
            ReadGif();
            GifInfoTest();
            GifCreatorTest();

            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        } // End Sub Main 
    } // End Class Program 
} // End Namespace AnimatedGif.Console 