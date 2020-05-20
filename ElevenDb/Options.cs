using System;
using System.Collections.Generic;
using System.Text;

namespace ElevenDb
{
    public class Options
    {
        public byte BlockSizeinKb { get; set; }

        public Options(byte BlockSizeinKb)
        {
            this.BlockSizeinKb = BlockSizeinKb;
        }


        internal static Options GetDefault()
        {
            return new Options(32);
        }
    }
}
