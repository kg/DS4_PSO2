using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS4_PSO2 {
    public struct ConvolutionKernel {
        public struct Pair<T> {
            public readonly T First, Second;

            public Pair (T a, T b) {
                First = a;
                Second = b;
            }
        }

        public readonly int Divisor;
        public readonly int Bias;
        public readonly int[] Values;
        public readonly int[] xOffsets, yOffsets;
        public readonly int Size, MaxOffset;

        public ConvolutionKernel (int divisor, int bias, params int[] values) {
            Divisor = divisor;
            Bias = bias;
            Size = (int)Math.Sqrt(values.Length);
            MaxOffset = (int)Math.Floor(Size / 2.0f);

            var nonZeroValues = new Dictionary<Pair<int>, int>();

            for (int y = 0, i = 0; y < Size; y++) {
                for (int x = 0; x < Size; x++, i++) {
                    var offset = new Pair<int>(x - MaxOffset, y - MaxOffset);
                    if (values[i] != 0)
                        nonZeroValues[offset] = values[i];
                }
            }

            Values = new int[nonZeroValues.Count];
            xOffsets = new int[nonZeroValues.Count];
            yOffsets = new int[nonZeroValues.Count];

            {
                int i = 0;
                foreach (var kvp in nonZeroValues) {
                    Values[i] = kvp.Value;
                    xOffsets[i] = kvp.Key.First;
                    yOffsets[i] = kvp.Key.Second;

                    i++;
                }
            }
        }

        // HACK: Modified specifically for the purpose of generating outlines. *shrug*
        public unsafe void Apply (byte * pSourceBytes, byte * pDestBytes, int width, int height) {
            var byteOffsets = stackalloc int[xOffsets.Length];

            unchecked {
                byte* pSourceAlpha = pSourceBytes + 3;

                for (int i = 0; i < Values.Length; i++) {
                    byteOffsets[i] = (xOffsets[i] + (yOffsets[i] * width)) * 4;
                }

                int topEdge = Size;
                int bottomEdge = height - Size;
                int leftEdge = Size;
                int rightEdge = width - Size;

                for (int y = 0; y < height; y++) {
                    bool yNearEdge = (y <= topEdge) || (y >= bottomEdge);
                    var rowOffset = (y * width * 4);
                    byte* pSourcePixel = pSourceBytes + rowOffset;
                    byte* pDestPixel = pDestBytes + rowOffset;

                    for (int x = 0, x4 = 0; x < width; x++, x4 += 4, pDestPixel += 4, pSourcePixel += 4) {
                        bool nearEdge = yNearEdge || (x <= leftEdge) || (x >= rightEdge);

                        int accumulator = Bias;

                        if (nearEdge) {
                            // Slow path; clip test against edges.

                            for (int k = 0; k < Values.Length; k++) {
                                var lx = x + xOffsets[k];
                                var ly = y + yOffsets[k];

                                if (lx < 0)
                                    lx = 0;
                                else if (lx >= width)
                                    lx = width - 1;

                                if (ly < 0)
                                    ly = 0;
                                else if (ly >= height)
                                    ly = height - 1;

                                var sampleOffset = ((ly * width) + lx) * 4;

                                accumulator += pSourceAlpha[sampleOffset] * Values[k];
                            }
                        } else {
                            var pixelOffset = rowOffset + x4;

                            // Fast path; just do dangerous pointer arithmetic.
                            for (int k = 0; k < Values.Length; k++)
                                accumulator += pSourceAlpha[pixelOffset + byteOffsets[k]] * Values[k];
                        }

                        var srcA = pSourcePixel[3];

                        accumulator /= Divisor;

                        accumulator += srcA;

                        if (accumulator < 0)
                            accumulator = 0;
                        else if (accumulator > 255)
                            accumulator = 255;

                        pDestPixel[0] = pDestPixel[1] = pDestPixel[2] = srcA;
                        pDestPixel[3] = (byte)(accumulator & 0xFF);
                    }
                }
            }
        }

        /*
        public void Convolve<TIn, TOut> (TextureContent input, TextureContent output, ContentProcessorContext context, Func<TIn, float> readPixel, Func<float, TOut> writePixel)
            where TIn : struct, System.IEquatable<TIn>
            where TOut : struct, System.IEquatable<TOut> {

            var inputFace = input.Faces[0];
            var outputFace = output.Faces[0];

            if (inputFace.Count != 1)
                throw new InvalidOperationException("Input must only have one mip level");
            if (outputFace.Count != 1)
                throw new InvalidOperationException("Output must only have one mip level");

            var inputMip = inputFace[0] as PixelBitmapContent<TIn>;
            var outputMip = outputFace[0] as PixelBitmapContent<TOut>;

            if ((inputMip.Width != outputMip.Width) || (inputMip.Height != outputMip.Height))
                throw new InvalidOperationException("Input and output sizes must match");

            TIn[] sourceData = new TIn[inputMip.Height * inputMip.Width];
            for (int y = 0; y < inputMip.Height; y++) {
                var row = inputMip.GetRow(y);
                Array.Copy(row, 0, sourceData, y * inputMip.Width, row.Length);
            }

            Func<int, int, float> getPixel;
            getPixel = (x, y) => {
                x = Arithmetic.Clamp(x, 0, inputMip.Width - 1);
                y = Arithmetic.Clamp(y, 0, inputMip.Height - 1);
                var color = sourceData[y * inputMip.Width + x];
                return readPixel(color);
            };

            for (int y = 0; y < outputMip.Height; y++) {
                var row = outputMip.GetRow(y);

                for (int x = 0; x < outputMip.Width; x++) {
                    float accumulator = Bias;

                    for (int k = 0; k < Values.Length; k++) {
                        var offset = Offsets[k];
                        accumulator += getPixel(x + offset.First, y + offset.Second)
                            * Values[k];
                    }

                    accumulator /= Divisor;

                    row[x] = writePixel(accumulator);
                }
            }
        }
         */
    }
}
