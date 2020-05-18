using System;
using System.Collections.Generic;
using System.Text;

namespace ElevenDb
{
    public class Options
    {
        public byte BlockSizeinKb { get; set; }
        public byte RecordSizeinKb { get; }

        public Options(byte BlockSizeinKb)
        {
            this.BlockSizeinKb = RecordSizeinKb;
        }


        internal static Options GetDefault()
        {
            return new Options(32);
        }
    }
}
