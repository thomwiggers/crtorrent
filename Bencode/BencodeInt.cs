using System;

namespace crtorrent.Bencode
{

    public class BencodeInt
    {
        public int Value
        {
            set;
            get;
        }
       
        public BencodeInt()
        {

        }

        public BencodeInt(int value)
        {
            this.Value = value;
        }

        public override string ToString()
        {
            if (Value != null)
            {
                return "i" + value.ToString() + "e";
            }
            return "";
        }

    }
}