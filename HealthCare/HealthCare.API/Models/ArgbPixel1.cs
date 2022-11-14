namespace HealthCare.API.Models
{
    public class ArgbPixel1
    {
        public byte RGB = 0;
        public byte RBG = 0;
        public byte BRG = 0;
        public byte GBR = 0;


        public ArgbPixel1()
        {

        }


        public ArgbPixel1(byte[] colorComponents)
        {
            RGB = colorComponents[0];
            RBG = colorComponents[1];
            BRG = colorComponents[2];
            GBR = colorComponents[3];
        }


        public byte[] GetColorBytes()
        {
            return new byte[] { RGB, RBG, BRG, GBR };
        }
    }
}
