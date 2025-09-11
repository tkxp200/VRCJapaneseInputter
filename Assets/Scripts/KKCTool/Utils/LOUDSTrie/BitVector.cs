using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Unity.Burst.Intrinsics;
using MemoryPack;

namespace LOUDSTrieUtil
{

    [MemoryPackable]
    public partial class BitVector
    {
        // max n = 2^32
        // lg(n)^2 = 1024 : ushort
        // lg(n)/2 = 16

        // const int BIG_BLOCK_SPLIT_SIZE = 8;
        // const int SMALL_BLOCK_SPLIT_SIZE = 4;
        const int BIG_BLOCK_SPLIT_SIZE = 1024;
        const int SMALL_BLOCK_SPLIT_SIZE = 16;

        const int SMALL_BLOCK_SIZE = BIG_BLOCK_SPLIT_SIZE / SMALL_BLOCK_SPLIT_SIZE;

        const int MAX_OUTPUT_BITSIZE = 1024;
        const int MAX_INDEX = 15;

        [MemoryPackOrder(0)]
        public ushort[] bitArray { get; }
        [MemoryPackOrder(1)]
        public uint size { get; }
        [MemoryPackOrder(2)]
        public int[] bigBlock { get; }
        [MemoryPackOrder(3)]
        public short[,] smallBlock { get; }

        public BitVector(List<ushort> bitArray)
        {
            this.bitArray = bitArray.ToArray();
            size = (uint)bitArray.Count * SMALL_BLOCK_SPLIT_SIZE;
            int bigBlockSize = bitArray.Count / SMALL_BLOCK_SIZE;
            bigBlock = new int[bigBlockSize + 1]; // bigBlock[0] = 0
            smallBlock = new short[bigBlockSize, SMALL_BLOCK_SIZE + 1]; //smallBlock[*, 0] = 0
            Build();
        }

        [MemoryPackConstructor]
        public BitVector(ushort[] bitArray, uint size, int[] bigBlock, short[,] smallBlock)
        {
            this.bitArray = bitArray;
            this.size = size;
            this.bigBlock = bigBlock;
            this.smallBlock = smallBlock;
        }


        private int Rank1(int position)
        {
            if ((uint)position > size) return -1;

            int count = 0;
            int bigBlockIndex = position / BIG_BLOCK_SPLIT_SIZE;
            count += bigBlock[bigBlockIndex];
            int smallBlockIndex = position % BIG_BLOCK_SPLIT_SIZE / SMALL_BLOCK_SPLIT_SIZE;
            count += smallBlock[bigBlockIndex, smallBlockIndex];
            uint mask = (uint)(1 << (position % SMALL_BLOCK_SPLIT_SIZE)) - 1;
            // count += BitOperations.PopCount(bitArray[position / SMALL_BLOCK_SPLIT_SIZE] & mask);
            count += X86.Popcnt.popcnt_u32(bitArray[position / SMALL_BLOCK_SPLIT_SIZE] & mask);
            return count;
        }

        public int Rank0(int position)
        {
            return position - Rank1(position);
        }

        public int Select1(int count)
        {
            if ((uint)count > size) return -1;
            if (count == 0) return -1;

            int position = 0;
            int remain = count;

            int left = -1;
            int right = bigBlock.Length;
            while (right - left > 1)
            {
                int mid = (left + right) / 2;

                if (bigBlock[mid] < remain) left = mid;
                else right = mid;
            }
            int bigBlockIndex = left;
            position += bigBlockIndex * BIG_BLOCK_SPLIT_SIZE;
            remain -= bigBlock[bigBlockIndex];

            left = -1;
            right = smallBlock.GetLength(1);
            while (right - left > 1)
            {
                int mid = (left + right) / 2;

                if (smallBlock[bigBlockIndex, mid] < remain) left = mid;
                else right = mid;
            }
            int smallBlockIndex = left;
            position += smallBlockIndex * SMALL_BLOCK_SPLIT_SIZE;
            remain -= smallBlock[bigBlockIndex, smallBlockIndex];

            var mask = bitArray[position / SMALL_BLOCK_SPLIT_SIZE];
            // for (remain--; remain > 0; remain--)
            // {
            //     mask &= (ushort)(mask - 1);
            // }
            uint value = 1u << (remain - 1);
            return (int)(position + X86.Bmi1.tzcnt_u32(X86.Bmi2.pdep_u32(value, mask)) + 1);
        }

        public bool GetBit(int position)
        {
            var index = position / SMALL_BLOCK_SPLIT_SIZE;
            var bitPos = position % SMALL_BLOCK_SPLIT_SIZE;
            return (bitArray[index] & (1 << bitPos)) != 0;
        }

        private void Build()
        {
            int count = 0;
            int smallBlockIndex = 0;
            int bigBlockIndex = 1;
            for (int i = 0; i < bitArray.Length; i++)
            {
                smallBlockIndex++;
                count += X86.Popcnt.popcnt_u32(bitArray[i]);
                smallBlock[bigBlockIndex - 1, smallBlockIndex] = (short)count;
                if (smallBlockIndex == SMALL_BLOCK_SIZE)
                {
                    bigBlock[bigBlockIndex] = bigBlock[bigBlockIndex - 1] + count;
                    bigBlockIndex++;
                    smallBlockIndex = 0;
                    count = 0;
                }
            }
        }

        public string Debug()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"BigBlockSize: {bigBlock.Length}");
            builder.AppendLine($"SmallBlockSize: {smallBlock.GetLength(0)},{smallBlock.GetLength(1)}");
            builder.AppendLine();

            if (size > MAX_OUTPUT_BITSIZE) return builder.ToString();

            builder.AppendLine($"BitVector Count: {bitArray.Length}");

            builder.AppendLine("BitVector:");
            for (int i = 0; i < MathF.Min(bitArray.Length, MAX_OUTPUT_BITSIZE); i++)
            {
                builder.Append(Convert.ToString(bitArray[i], 2).PadLeft(MAX_INDEX + 1, '0').Reverse().ToArray());
                builder.AppendLine();
            }
            builder.AppendLine();
            builder.AppendLine();

            builder.AppendLine("BigBlock:");
            builder.AppendJoin(",", bigBlock).AppendLine();
            builder.AppendLine();

            builder.AppendLine("smallBlock:");
            for (int i = 0; i < smallBlock.GetLength(0); i++)
            {
                var rowArray = Enumerable.Range(0, smallBlock.GetLength(1))
                                        .Select(j => smallBlock[i, j]);
                builder.AppendJoin(",", rowArray).AppendLine();
            }
            builder.AppendLine();

            return builder.ToString();
        }
    }

    public class BitVectorBuilder
    {
        const int MAX_OUTPUT_BITSIZE = 128;
        const int MAX_INDEX = 15;
        const int BIG_BLOCK_SPLIT_SIZE = 1024;
        const int SMALL_BLOCK_SPLIT_SIZE = 16;
        const ushort MAX_VALUE = ushort.MaxValue;

        const int SMALL_BLOCK_SIZE = BIG_BLOCK_SPLIT_SIZE / SMALL_BLOCK_SPLIT_SIZE; //64

        private readonly List<ushort> bitVectorList;
        private int bitPos;
        private int listIndex = -1;
        private int capacity;
        private ushort bitVector = MAX_VALUE;

        public BitVectorBuilder()
        {
            capacity = SMALL_BLOCK_SIZE;
            bitVectorList = new(capacity);

            // AddRoot();
        }

        public void AddRoot()
        {
            Add(false);
            Add(true);
        }

        public void Add(bool value)
        {
            if (!value) bitVector &= (ushort)~(1 << bitPos);

            bitPos++;

            if (bitPos > MAX_INDEX)
            {
                listIndex++;
                if (listIndex >= capacity)
                {
                    capacity += SMALL_BLOCK_SIZE;
                    bitVectorList.Capacity = capacity;
                }

                bitVectorList.Add(bitVector);
                bitVector = MAX_VALUE;
                bitPos = 0;
            }
        }

        public BitVector Build()
        {
            if (bitPos != 0) bitVectorList.Add(bitVector);

            FillToCapacity();

            return new BitVector(bitVectorList);
        }

        public void FillToCapacity()
        {
            while (bitVectorList.Count < capacity)
            {
                bitVectorList.Add(MAX_VALUE);
            }
        }

        private string Debug()
        {

            var builder = new StringBuilder();
            if (bitVectorList.Count > MAX_OUTPUT_BITSIZE)
                builder.AppendLine("The BitVector is too large. Only a portion will be displayed.\n");

            builder.AppendLine($"BitVector Count: {bitVectorList.Count}");

            builder.AppendLine("BitVector:");
            for (int i = 0; i < MathF.Min(bitVectorList.Count, MAX_OUTPUT_BITSIZE); i++)
            {
                builder.AppendLine(Convert.ToString(bitVectorList[i], 2).PadLeft(16, '0'));
            }
            if (bitPos != 0) builder.AppendLine(Convert.ToString(bitVector, 2).PadLeft(16, '0'));

            return builder.ToString();
        }

    }
}
