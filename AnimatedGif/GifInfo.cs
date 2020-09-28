
using System;
using System.Runtime.InteropServices;

namespace AnimatedGif
{


    // https://dejanstojanovic.net/aspnet/2018/march/getting-gif-image-information-using-c/
    public class GifInfo
    {

        private System.IO.FileInfo fileInfo;
        private System.Collections.Generic.IList<System.Drawing.Image> frames;
        private System.Drawing.Size size;
        private bool animated;
        private bool loop;
        private System.TimeSpan animationDuration;
    

        public System.IO.FileInfo FileInfo
        {
            get
            {
                return this.fileInfo;
            }
        }

        public System.Collections.Generic.IList<System.Drawing.Image> Frames
        {
            get
            {
                return this.frames;
            }
        }

        public System.Drawing.Size Size
        {
            get
            {
                return this.size;
            }
        }

        public bool Animated
        {
            get
            {
                return this.animated;
            }
        }

        public bool Loop
        {
            get
            {
                return this.loop;
            }
        }

        public System.TimeSpan AnimationDuration
        {
            get
            {
                return this.animationDuration;
            }
        }


        public GifInfo(string filePath)
        {
            bool isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            
            if (System.IO.File.Exists(filePath))
            {
                using (System.Drawing.Image image = System.Drawing.Image.FromFile(filePath))
                {
                    this.size = new System.Drawing.Size(image.Width, image.Height);

                    if (image.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Gif))
                    {
                        this.frames = new System.Collections.Generic.List<System.Drawing.Image>();
                        this.fileInfo = new System.IO.FileInfo(filePath);

                        if (System.Drawing.ImageAnimator.CanAnimate(image))
                        {
                            // Get frames
                            System.Drawing.Imaging.FrameDimension dimension = 
                                new System.Drawing.Imaging.FrameDimension(image.FrameDimensionsList[0]);

                            int frameCount = image.GetFrameCount(dimension);

                            int index = 0;
                            int duration = 0;
                            for (int i = 0; i < frameCount; i++)
                            {
                                image.SelectActiveFrame(dimension, i);
                                System.Drawing.Image frame = image.Clone() as System.Drawing.Image;
                                frames.Add(frame);
                                
                                // https://docs.microsoft.com/en-us/dotnet/api/system.drawing.imaging.propertyitem.id?view=dotnet-plat-ext-3.1
                                // 0x5100	PropertyTagFrameDelay
                                byte[] propertyArray = image.GetPropertyItem(20736).Value;

                                int delay = 0;
                                
                                if(isWindows)        
                                 delay = System.BitConverter.ToInt32(propertyArray, index) * 10;
                                else
                                    delay = System.BitConverter.ToInt32(propertyArray, 0) * 10;
                                
                                duration += (delay < 100 ? 100 : delay);
                                
                                index += 4;
                            } // Next i 

                            this.animationDuration = System.TimeSpan.FromMilliseconds(duration);
                            this.animated = true;
                            this.loop = System.BitConverter.ToInt16(image.GetPropertyItem(20737).Value, 0) != 1;
                        }
                        else
                        {
                            this.frames.Add(image.Clone() as System.Drawing.Image);
                        }

                    }
                    else
                    {
                        throw new System.FormatException("Not valid GIF image format");
                    }
                } // End Using image 

            } // End if (System.IO.File.Exists(filePath)) 

        } // End Constructor 


    } // End Class GifInfo 


} // End Namespace GifInfo 
