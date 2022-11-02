﻿using Enums;

namespace SwitchGiftDataManager.Core
{
    internal abstract class Wondercard
    {
        protected const int MaxItemCount = 5;

        public Games Game { get; }
        public ushort WCID { get; protected set; }
        public object? Type { get; protected set; }
        public object? Content { get; protected set; }
        public byte[]? Data { get; protected set; }

        public Wondercard(ReadOnlySpan<byte> data)
        {
            Game = (WondercardSize)data.Length switch
            {
                WondercardSize.WB7 => Games.LGPE,
                WondercardSize.WC8 => Games.SWSH,
                WondercardSize.WB8 => Games.BDSP,
                WondercardSize.WA8 => Games.PLA,
                WondercardSize.WC9 => Games.SCVI,
                _ => Games.None,
            };
            Data = data.ToArray();
        }

        public bool IsValid()
        {
            if (WCID <= 0)
                return false;

            if (Content is null)
                return false;

            if (!IsChecksumValid())
                UpdateChecksum();

            return true;
        }

        public ReadOnlySpan<byte> CalcMetaChecksum() => ChecksumCalculator.CalcReverseMD5(Data!);

        public abstract bool IsChecksumValid();

        public abstract void UpdateChecksum();

        public abstract void SetID(ushort wcid);

        public static WondercardSize GetSize(Games game)
        {
            return game switch
            {
                Games.LGPE => WondercardSize.WB7,
                Games.SWSH => WondercardSize.WC8,
                Games.BDSP => WondercardSize.WB8,
                Games.PLA => WondercardSize.WA8,
                Games.SCVI => WondercardSize.WC9,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }
}
