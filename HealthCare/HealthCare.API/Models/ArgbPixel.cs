namespace HealthCare.API.Models
{
    public class ArgbPixel
    {
        public byte blue = 0;
        public byte green = 0;
        public byte red = 0;
        public byte alpha = 0;


        public ArgbPixel()
        {

        }


        public ArgbPixel(byte[] colorComponents)
        {
            blue = colorComponents[0];
            green = colorComponents[1];
            red = colorComponents[2];
            alpha = colorComponents[3];
        }


        public byte[] GetColorBytes()
        {
            return new byte[] { blue, green, red, alpha };
        }
    }
}
