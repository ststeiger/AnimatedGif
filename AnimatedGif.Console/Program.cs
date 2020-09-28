
using SixLabors.ImageSharp; // Needed for GetGifMetadata 
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Processing; // Neeeded for mutate


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

                using (System.IO.Stream outStream = new System.IO.MemoryStream())
                {
                    using (SixLabors.ImageSharp.Image image = 
                         SixLabors.ImageSharp.Image.Load(inStream 
                        ,out SixLabors.ImageSharp.Formats.IImageFormat format) 
                    )
                    {
                        SixLabors.ImageSharp.Image clone = image.Clone(
                            i => i.Resize(width, height)
                                .Crop(new SixLabors.ImageSharp.Rectangle(x, y, cropWidth, cropHeight))
                        );

                        SixLabors.ImageSharp.Formats.IImageEncoder form = 
                            new SixLabors.ImageSharp.Formats.Png.PngEncoder();

                        clone.Save(outStream, form);
                    } // End Using image 

                } // End Using outStream 

            } // End Using inStream 

        } // End Sub ResizeCrop 


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

            SixLabors.ImageSharp.Image gif = new SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32>(480, 480);

            SixLabors.ImageSharp.Image foa = SixLabors.ImageSharp.Image.Load("FOA.jpg");
            foa.Mutate(o => o.Resize(new SixLabors.ImageSharp.Size(150, 135)));
            // foa.SaveAsPng("foa.png");

            SixLabors.ImageSharp.Point pt = new SixLabors.ImageSharp.Point(290, 45);

            for (int i = 0; i < 70; ++i)
            {
                string fn = "frame-" + i.ToString().PadLeft(2, '0') + ".png";
                using (SixLabors.ImageSharp.Image frameImage = SixLabors.ImageSharp.Image.Load(fn))
                {
                    // https://stackoverflow.com/questions/50860392/how-to-combine-two-images
                    frameImage.Mutate(imageProcess => imageProcess.DrawImage(foa, pt, 1f));
                    
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
            using (SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(path))
            {
                SixLabors.ImageSharp.Formats.Gif.GifMetadata imageMetadata = 
                    image.Metadata.GetFormatMetadata(
                        SixLabors.ImageSharp.Formats.Gif.GifFormat.Instance
                );

                // SixLabors.ImageSharp.Formats.Gif.GifMetadata gifMeta = image.Metadata.GetGifMetadata();

                for (int i = 0; i < image.Frames.Count; ++i)
                {
                    // image.Frames.ExportFrame()

                    using (SixLabors.ImageSharp.Image frameImage = image.Frames.CloneFrame(i))
                    {
                        string outPath = "frame-" + i.ToString().PadLeft(2, '0') + ".png";
                        
                        using (System.IO.Stream fs = System.IO.File.OpenWrite(outPath))
                        {
                            // we include all metadata from the original image;
                            frameImage.Save(fs, new SixLabors.ImageSharp.Formats.Png.PngEncoder());
                        } 

                    } // End Using frameImage 

                    SixLabors.ImageSharp.Metadata.ImageFrameMetadata frameMetaData = 
                        image.Frames[i].Metadata;

                    SixLabors.ImageSharp.Formats.Gif.GifFrameMetadata frameGifMetaData = 
                        frameMetaData.GetFormatMetadata(
                            SixLabors.ImageSharp.Formats.Gif.GifFormat.Instance
                    );

                    System.Console.WriteLine(frameGifMetaData.FrameDelay);
                } // Next i 

            } // End Using image 

        } // End Sub ReadGif 


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
            using (AnimatedGifCreator gif = AnimatedGif.Create("gif.gif", 33, -1))
            {
                System.Drawing.Image img1 = System.Drawing.Image.FromFile("img1.png");
                gif.AddFrame(img1, delay: 2000, quality: GifQuality.Bit8);
                System.Drawing.Image img2 = System.Drawing.Image.FromFile("img2.png");
                gif.AddFrame(img2, delay: 2000, quality: GifQuality.Bit8);
                System.Drawing.Image img3 = System.Drawing.Image.FromFile("img3.png");
                gif.AddFrame(img3, delay: 2000, quality: GifQuality.Bit8);
            } // End Using gif 

        } // End Sub GifCreatorTest 


        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static System.Drawing.Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
        {
            System.Drawing.Rectangle destRect = new System.Drawing.Rectangle(0, 0, width, height);
            System.Drawing.Bitmap destImage = new System.Drawing.Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                using (System.Drawing.Imaging.ImageAttributes wrapMode = 
                    new System.Drawing.Imaging.ImageAttributes())
                {
                    wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, System.Drawing.GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public static void CreateGif()
        {
            // topLeft 290/45
            // topRight 440/45
            // bottomLeft 290/180
            // bottomRight 440/180

            // x = 290
            // y = 45
            // w = 440 - 290 = 150
            // h = 180 -  45 = 135
            string path = @"output2.gif";

            using (System.Drawing.Image img = System.Drawing.Image.FromFile("FOA.jpg"))
            {

                using (System.Drawing.Image foa = ResizeImage(img, 150, 135))
                {

                    using (AnimatedGifCreator gif = AnimatedGif.Create(path, 33, -1))
                    {
                        // SixLabors.ImageSharp.Point pt = new SixLabors.ImageSharp.Point(290, 45);

                        for (int i = 0; i < 69; ++i)
                        {
                            string fn = "sample-" + i.ToString().PadLeft(2, '0') + ".png";

                            using (System.Drawing.Image imgFrame = System.Drawing.Image.FromFile(fn))
                            {

                                using (System.Drawing.Graphics gr = 
                                    System.Drawing.Graphics.FromImage(imgFrame))
                                {
                                    gr.DrawImage(foa, new System.Drawing.Rectangle(290, 45, foa.Width, foa.Height));
                                } // End using gr 

                                gif.AddFrame(imgFrame, delay: 60, quality: GifQuality.Bit8);
                            } // End Using img 

                        } // Next i 

                    } // End Using gif 

                } // End Using foa 

            } // End Using img 

        } // End Sub WriteGif


        public static void Main(string[] args)
        {
            ReadGif();
            WriteGif();

            CreateGif();
            GifInfoTest();
            GifCreatorTest();

            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        } // End Sub Main 


    } // End Class Program 


} // End Namespace AnimatedGif.Console 
