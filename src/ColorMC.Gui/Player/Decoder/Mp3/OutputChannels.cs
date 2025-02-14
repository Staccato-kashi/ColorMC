﻿namespace ColorMC.Gui.Player.Decoder.Mp3;

public class OutputChannels
{
    /**
     * Flag to indicate output should include both channels.
     */
    public const int BOTH_CHANNELS = 0;

    /**
     * Flag to indicate output should include the left channel only.
     */
    public const int LEFT_CHANNEL = 1;

    /**
     * Flag to indicate output should include the right channel only.
     */
    public const int RIGHT_CHANNEL = 2;

    /**
     * Flag to indicate output is mono.
     */
    public const int DOWNMIX_CHANNELS = 3;


    //private readonly int outputChannels;

    //private OutputChannels(int channels)
    //{
    //    outputChannels = channels;

    //    if (channels < 0 || channels > 3)
    //        throw new ArgumentException("channels");
    //}


    //public override bool Equals(object? o)
    //{
    //    bool equals = false;

    //    if (o is OutputChannels oc) 
    //    {
    //        equals = (oc.outputChannels == outputChannels);
    //    }

    //    return equals;
    //}

    //public int HashCode()
    //{
    //    return outputChannels;
    //}

}

