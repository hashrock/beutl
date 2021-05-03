﻿using System;
using System.IO;

using FFmpeg.AutoGen;

namespace FFmpegTest
{
    unsafe class Program
    {
#pragma warning disable CS0618 // 型またはメンバーが旧型式です
        static int check_sample_fmt(AVCodec* codec, AVSampleFormat sample_fmt)
        {
            AVSampleFormat* p = codec->sample_fmts;
            while (*p != AVSampleFormat.AV_SAMPLE_FMT_NONE)
            {
                if (*p == sample_fmt)
                    return 1;
                p++;
            }
            return 0;
        }
        /* just pick the highest supported samplerate */
        static int select_sample_rate(AVCodec* codec)
        {
            int* p;
            int best_samplerate = 0;
            if (codec->supported_samplerates == null)
                return 44100;
            p = codec->supported_samplerates;
            while (*p != 0)
            {
                if (best_samplerate == 0 || Math.Abs(44100 - *p) < Math.Abs(44100 - best_samplerate))
                    best_samplerate = *p;
                p++;
            }
            return best_samplerate;
        }
        /* select layout with the highest channel count */
        static ulong select_channel_layout(AVCodec* codec)
        {
            ulong* p;
            ulong best_ch_layout = 0;
            int best_nb_channels = 0;
            if (codec->channel_layouts == null)
                return ffmpeg.AV_CH_LAYOUT_STEREO;
            p = codec->channel_layouts;
            while (*p != 0)
            {
                int nb_channels = ffmpeg.av_get_channel_layout_nb_channels(*p);
                if (nb_channels > best_nb_channels)
                {
                    best_ch_layout = *p;
                    best_nb_channels = nb_channels;
                }
                p++;
            }
            return best_ch_layout;
        }
        static void encode(AVCodecContext* ctx, AVFrame* frame, AVPacket* pkt, Stream output)
        {
            int ret;
            /* send the frame for encoding */
            ret = ffmpeg.avcodec_send_frame(ctx, frame);
            if (ret < 0)
            {
                Console.Error.WriteLine("Error sending the frame to the encoder");
                Environment.Exit(1);
            }
            /* read all the available output packets (in general there may be any
             * number of them */
            while (ret >= 0)
            {
                ret = ffmpeg.avcodec_receive_packet(ctx, pkt);
                if (ret == ffmpeg.AVERROR(ffmpeg.EAGAIN) || ret == ffmpeg.AVERROR_EOF)
                    return;
                else if (ret < 0)
                {
                    Console.Error.WriteLine("Error encoding audio frame\n");
                    Environment.Exit(1);
                }
                output.Write(new(pkt->data, pkt->size));
                ffmpeg.av_packet_unref(pkt);
            }
        }


        static int Main(string[] args)
        {
            ffmpeg.RootPath = @"C:\github.com\b-editor\ffmpeg\ffmpeg";

            string filename;
            AVCodec* codec;
            AVCodecContext* c = null;
            AVFrame* frame;
            AVPacket* pkt;
            int i, j, k, ret;
            Stream f;
            ushort* samples;
            float t, tincr;

            filename = @"E:\yuuto\Downloads\a.mp3";
            /* find the MP2 encoder */
            codec = ffmpeg.avcodec_find_encoder(AVCodecID.AV_CODEC_ID_MP2);
            if (codec == null)
            {
                Console.Error.WriteLine("Codec not found");
                return 1;
            }
            c = ffmpeg.avcodec_alloc_context3(codec);
            if (c == null)
            {
                Console.Error.WriteLine("Could not allocate audio codec context");
                return 1;
            }
            /* put sample parameters */
            c->bit_rate = 64000;
            /* check that the encoder supports s16 pcm input */
            c->sample_fmt = AVSampleFormat.AV_SAMPLE_FMT_S16;
            if (check_sample_fmt(codec, c->sample_fmt) == 0)
            {
                Console.Error.WriteLine("Encoder does not support sample format {0}",
                        ffmpeg.av_get_sample_fmt_name(c->sample_fmt));
                return 1;
            }
            /* select other audio parameters supported by the encoder */
            c->sample_rate = select_sample_rate(codec);
            c->channel_layout = select_channel_layout(codec);
            c->channels = ffmpeg.av_get_channel_layout_nb_channels(c->channel_layout);
            /* open it */
            if (ffmpeg.avcodec_open2(c, codec, null) < 0)
            {
                Console.Error.WriteLine("Could not open codec");
                return 1;
            }
            f = new FileStream(filename, FileMode.Create);
            /* packet for holding encoded output */
            pkt = ffmpeg.av_packet_alloc();
            if (pkt==null)
            {
                Console.Error.WriteLine("could not allocate the packet");
                return 1;
            }
            /* frame containing input raw audio */
            frame = ffmpeg.av_frame_alloc();
            if (frame==null)
            {
                Console.Error.WriteLine("Could not allocate audio frame");
                return 1;
            }
            frame->nb_samples = c->frame_size;
            frame->format = (int)c->sample_fmt;
            frame->channel_layout = c->channel_layout;
            /* allocate the data buffers */
            ret = ffmpeg.av_frame_get_buffer(frame, 0);
            if (ret < 0)
            {
                Console.Error.WriteLine("Could not allocate audio data buffers");
                return 1;
            }
            /* encode a single tone sound */
            t = 0;
            tincr = (float)(2 * Math.PI * 440.0 / c->sample_rate);
            for (i = 0; i < 200; i++)
            {
                /* make sure the frame is writable -- makes a copy if the encoder
                 * kept a reference internally */
                ret = ffmpeg.av_frame_make_writable(frame);
                if (ret < 0)
                    return 1;
                samples = (ushort*)frame->data[0];
                for (j = 0; j < c->frame_size; j++)
                {
                    samples[2 * j] = (ushort)(Math.Sin(t) * 10000);
                    for (k = 1; k < c->channels; k++)
                        samples[2 * j + k] = samples[2 * j];
                    t += tincr;
                }
                encode(c, frame, pkt, f);
            }
            /* flush the encoder */
            encode(c, null, pkt, f);
            f.Dispose();
            ffmpeg.av_frame_free(&frame);
            ffmpeg.av_packet_free(&pkt);
            ffmpeg.avcodec_free_context(&c);
            return 0;
        }
#pragma warning restore CS0618 // 型またはメンバーが旧型式です
    }
}
