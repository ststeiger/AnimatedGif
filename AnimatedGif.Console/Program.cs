
namespace AnimatedGif.Console
{


    // https://stackoverflow.com/questions/64023538/is-it-possible-to-merge-a-image1-gift-and-image2-jpg-with-net-or-javascript
    // https://www.evoketechnologies.com/blog/code-review-checklist-perform-effective-code-reviews/
    // https://www.michaelagreiler.com/code-review-checklist-2/
    // https://de.wikipedia.org/wiki/Zahlennamen
    // https://en.wikipedia.org/wiki/Names_of_large_numbers

    public class Program
    {


        public static void GifInfoTest()
        {
            string path = @"sample.gif";

            GifInfo info = new GifInfo(path);
            for (int i = 0; i < info.Frames.Count - 1; i++)
            {
                string frameFilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(path), $"{System.IO.Path.GetFileNameWithoutExtension(path)}-{i.ToString().PadLeft(2, '0')}.png");
                System.Drawing.Image frame = info.Frames[i];

                System.Console.WriteLine(frameFilePath);

                frame.Save(frameFilePath, System.Drawing.Imaging.ImageFormat.Png);
            } // Next i 

        } // End Sub GifInfoTest 


        public static void Main(string[] args)
        {
            GifInfoTest();

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

            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        } // End Sub Main 


    } // End Class Program 


} // End Namespace AnimatedGif.Console 
