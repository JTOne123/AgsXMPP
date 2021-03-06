// Inflater.cs
//
// Copyright (C) 2001 Mike Krueger
// Copyright (C) 2004 John Reilly
//
// This file was translated from java, it was part of the GNU Classpath
// Copyright (C) 2001 Free Software Foundation, Inc.
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// Linking this library statically or dynamically with other modules is
// making a combined work based on this library.  Thus, the terms and
// conditions of the GNU General Public License cover the whole
// combination.
// 
// As a special exception, the copyright holders of this library give you
// permission to link this library with independent modules to produce an
// executable, regardless of the license terms of these independent
// modules, and to copy and distribute the resulting executable under
// terms of your choice, provided that you also meet, for each linked
// independent module, the terms and conditions of the license of that
// module.  An independent module is a module which is not derived from
// or based on this library.  If you modify this library, you may extend
// this exception to your version of the library, but you are not
// obligated to do so.  If you do not wish to do so, delete this
// exception statement from your version.

using System;

using AgsXMPP.IO.Compression.Checksums;
using AgsXMPP.IO.Compression.Streams;

namespace AgsXMPP.IO.Compression
{

	/// <summary>
	/// Inflater is used to decompress data that has been compressed according
	/// to the "deflate" standard described in rfc1951.
	/// 
	/// By default Zlib (rfc1950) headers and footers are expected in the input.
	/// You can use constructor <code> public Inflater(bool noHeader)</code> passing true
	/// if there is no Zlib header information
	///
	/// The usage is as following.  First you have to set some input with
	/// <code>setInput()</code>, then inflate() it.  If inflate doesn't
	/// inflate any bytes there may be three reasons:
	/// <ul>
	/// <li>needsInput() returns true because the input buffer is empty.
	/// You have to provide more input with <code>setInput()</code>.
	/// NOTE: needsInput() also returns true when, the stream is finished.
	/// </li>
	/// <li>needsDictionary() returns true, you have to provide a preset
	///    dictionary with <code>setDictionary()</code>.</li>
	/// <li>finished() returns true, the inflater has finished.</li>
	/// </ul>
	/// Once the first output byte is produced, a dictionary will not be
	/// needed at a later stage.
	///
	/// author of the original java version : John Leuner, Jochen Hoenicke
	/// </summary>
	public class Inflater
	{
		/// <summary>
		/// Copy lengths for literal codes 257..285
		/// </summary>
		static int[] CPLENS = {
								 3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 15, 17, 19, 23, 27, 31,
								 35, 43, 51, 59, 67, 83, 99, 115, 131, 163, 195, 227, 258
							  };

		/// <summary>
		/// Extra bits for literal codes 257..285
		/// </summary>
		static int[] CPLEXT = {
								 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2,
								 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 0
							  };

		/// <summary>
		/// Copy offsets for distance codes 0..29
		/// </summary>
		static int[] CPDIST = {
								1, 2, 3, 4, 5, 7, 9, 13, 17, 25, 33, 49, 65, 97, 129, 193,
								257, 385, 513, 769, 1025, 1537, 2049, 3073, 4097, 6145,
								8193, 12289, 16385, 24577
							  };

		/// <summary>
		/// Extra bits for distance codes
		/// </summary>
		static int[] CPDEXT = {
								0, 0, 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6,
								7, 7, 8, 8, 9, 9, 10, 10, 11, 11,
								12, 12, 13, 13
							  };

		/// <summary>
		/// These are the possible states for an inflater
		/// </summary>
		const int DECODE_HEADER = 0;
		const int DECODE_DICT = 1;
		const int DECODE_BLOCKS = 2;
		const int DECODE_STORED_LEN1 = 3;
		const int DECODE_STORED_LEN2 = 4;
		const int DECODE_STORED = 5;
		const int DECODE_DYN_HEADER = 6;
		const int DECODE_HUFFMAN = 7;
		const int DECODE_HUFFMAN_LENBITS = 8;
		const int DECODE_HUFFMAN_DIST = 9;
		const int DECODE_HUFFMAN_DISTBITS = 10;
		const int DECODE_CHKSUM = 11;
		const int FINISHED = 12;

		/// <summary>
		/// This variable contains the current state.
		/// </summary>
		int mode;

		/// <summary>
		/// The adler checksum of the dictionary or of the decompressed
		/// stream, as it is written in the header resp. footer of the
		/// compressed stream. 
		/// Only valid if mode is DECODE_DICT or DECODE_CHKSUM.
		/// </summary>
		int readAdler;

		/// <summary>
		/// The number of bits needed to complete the current state.  This
		/// is valid, if mode is DECODE_DICT, DECODE_CHKSUM,
		/// DECODE_HUFFMAN_LENBITS or DECODE_HUFFMAN_DISTBITS.
		/// </summary>
		int neededBits;
		int repLength;
		int repDist;
		int uncomprLen;

		/// <summary>
		/// True, if the last block flag was set in the last block of the
		/// inflated stream.  This means that the stream ends after the
		/// current block.
		/// </summary>
		bool isLastBlock;

		/// <summary>
		/// The total number of inflated bytes.
		/// </summary>
		int totalOut;

		/// <summary>
		/// The total number of bytes set with setInput().  This is not the
		/// value returned by the TotalIn property, since this also includes the
		/// unprocessed input.
		/// </summary>
		int totalIn;

		/// <summary>
		/// This variable stores the noHeader flag that was given to the constructor.
		/// True means, that the inflated stream doesn't contain a Zlib header or 
		/// footer.
		/// </summary>
		bool noHeader;

		StreamManipulator input;
		OutputWindow outputWindow;
		InflaterDynHeader dynHeader;
		InflaterHuffmanTree litlenTree, distTree;
		Adler32 adler;

		/// <summary>
		/// Creates a new inflater or RFC1951 decompressor
		/// RFC1950/Zlib headers and footers will be expected in the input data
		/// </summary>
		public Inflater() : this(false)
		{
		}

		/// <summary>
		/// Creates a new inflater.
		/// </summary>
		/// <param name="noHeader">
		/// True if no RFC1950/Zlib header and footer fields are expected in the input data
		/// 
		/// This is used for GZIPed/Zipped input.
		/// 
		/// For compatibility with
		/// Sun JDK you should provide one byte of input more than needed in
		/// this case.
		/// </param>
		public Inflater(bool noHeader)
		{
			this.noHeader = noHeader;
			this.adler = new Adler32();
			this.input = new StreamManipulator();
			this.outputWindow = new OutputWindow();
			this.mode = noHeader ? DECODE_BLOCKS : DECODE_HEADER;
		}

		/// <summary>
		/// Resets the inflater so that a new stream can be decompressed.  All
		/// pending input and output will be discarded.
		/// </summary>
		public void Reset()
		{
			this.mode = this.noHeader ? DECODE_BLOCKS : DECODE_HEADER;
			this.totalIn = this.totalOut = 0;
			this.input.Reset();
			this.outputWindow.Reset();
			this.dynHeader = null;
			this.litlenTree = null;
			this.distTree = null;
			this.isLastBlock = false;
			this.adler.Reset();
		}

		/// <summary>
		/// Decodes a zlib/RFC1950 header.
		/// </summary>
		/// <returns>
		/// False if more input is needed.
		/// </returns>
		/// <exception cref="SharpZipBaseException">
		/// The header is invalid.
		/// </exception>
		private bool DecodeHeader()
		{
			var header = this.input.PeekBits(16);
			if (header < 0)
			{
				return false;
			}
			this.input.DropBits(16);

			/* The header is written in "wrong" byte order */
			header = ((header << 8) | (header >> 8)) & 0xffff;
			if (header % 31 != 0)
			{
				throw new SharpZipBaseException("Header checksum illegal");
			}

			if ((header & 0x0f00) != (Deflater.DEFLATED << 8))
			{
				throw new SharpZipBaseException("Compression Method unknown");
			}

			/* Maximum size of the backwards window in bits.
			* We currently ignore this, but we could use it to make the
			* inflater window more space efficient. On the other hand the
			* full window (15 bits) is needed most times, anyway.
			int max_wbits = ((header & 0x7000) >> 12) + 8;
			*/

			if ((header & 0x0020) == 0)
			{ // Dictionary flag?
				this.mode = DECODE_BLOCKS;
			}
			else
			{
				this.mode = DECODE_DICT;
				this.neededBits = 32;
			}
			return true;
		}

		/// <summary>
		/// Decodes the dictionary checksum after the deflate header.
		/// </summary>
		/// <returns>
		/// False if more input is needed.
		/// </returns>
		private bool DecodeDict()
		{
			while (this.neededBits > 0)
			{
				var dictByte = this.input.PeekBits(8);
				if (dictByte < 0)
				{
					return false;
				}
				this.input.DropBits(8);
				this.readAdler = (this.readAdler << 8) | dictByte;
				this.neededBits -= 8;
			}
			return false;
		}

		/// <summary>
		/// Decodes the huffman encoded symbols in the input stream.
		/// </summary>
		/// <returns>
		/// false if more input is needed, true if output window is
		/// full or the current block ends.
		/// </returns>
		/// <exception cref="SharpZipBaseException">
		/// if deflated stream is invalid.
		/// </exception>
		private bool DecodeHuffman()
		{
			var free = this.outputWindow.GetFreeSpace();
			while (free >= 258)
			{
				int symbol;
				switch (this.mode)
				{
					case DECODE_HUFFMAN:
						/* This is the inner loop so it is optimized a bit */
						while (((symbol = this.litlenTree.GetSymbol(this.input)) & ~0xff) == 0)
						{
							this.outputWindow.Write(symbol);
							if (--free < 258)
							{
								return true;
							}
						}

						if (symbol < 257)
						{
							if (symbol < 0)
							{
								return false;
							}
							else
							{
								/* symbol == 256: end of block */
								this.distTree = null;
								this.litlenTree = null;
								this.mode = DECODE_BLOCKS;
								return true;
							}
						}

						try
						{
							this.repLength = CPLENS[symbol - 257];
							this.neededBits = CPLEXT[symbol - 257];
						}
						catch (Exception)
						{
							throw new SharpZipBaseException("Illegal rep length code");
						}
						goto case DECODE_HUFFMAN_LENBITS; /* fall through */

					case DECODE_HUFFMAN_LENBITS:
						if (this.neededBits > 0)
						{
							this.mode = DECODE_HUFFMAN_LENBITS;
							var i = this.input.PeekBits(this.neededBits);
							if (i < 0)
							{
								return false;
							}
							this.input.DropBits(this.neededBits);
							this.repLength += i;
						}
						this.mode = DECODE_HUFFMAN_DIST;
						goto case DECODE_HUFFMAN_DIST;/* fall through */

					case DECODE_HUFFMAN_DIST:
						symbol = this.distTree.GetSymbol(this.input);
						if (symbol < 0)
						{
							return false;
						}

						try
						{
							this.repDist = CPDIST[symbol];
							this.neededBits = CPDEXT[symbol];
						}
						catch (Exception)
						{
							throw new SharpZipBaseException("Illegal rep dist code");
						}

						goto case DECODE_HUFFMAN_DISTBITS;/* fall through */

					case DECODE_HUFFMAN_DISTBITS:
						if (this.neededBits > 0)
						{
							this.mode = DECODE_HUFFMAN_DISTBITS;
							var i = this.input.PeekBits(this.neededBits);
							if (i < 0)
							{
								return false;
							}
							this.input.DropBits(this.neededBits);
							this.repDist += i;
						}

						this.outputWindow.Repeat(this.repLength, this.repDist);
						free -= this.repLength;
						this.mode = DECODE_HUFFMAN;
						break;

					default:
						throw new SharpZipBaseException("Inflater unknown mode");
				}
			}
			return true;
		}

		/// <summary>
		/// Decodes the adler checksum after the deflate stream.
		/// </summary>
		/// <returns>
		/// false if more input is needed.
		/// </returns>
		/// <exception cref="SharpZipBaseException">
		/// If checksum doesn't match.
		/// </exception>
		private bool DecodeChksum()
		{
			while (this.neededBits > 0)
			{
				var chkByte = this.input.PeekBits(8);
				if (chkByte < 0)
				{
					return false;
				}
				this.input.DropBits(8);
				this.readAdler = (this.readAdler << 8) | chkByte;
				this.neededBits -= 8;
			}
			if ((int)this.adler.Value != this.readAdler)
			{
				throw new SharpZipBaseException("Adler chksum doesn't match: " + (int)this.adler.Value + " vs. " + this.readAdler);
			}
			this.mode = FINISHED;
			return false;
		}

		/// <summary>
		/// Decodes the deflated stream.
		/// </summary>
		/// <returns>
		/// false if more input is needed, or if finished.
		/// </returns>
		/// <exception cref="SharpZipBaseException">
		/// if deflated stream is invalid.
		/// </exception>
		private bool Decode()
		{
			switch (this.mode)
			{
				case DECODE_HEADER:
					return this.DecodeHeader();
				case DECODE_DICT:
					return this.DecodeDict();
				case DECODE_CHKSUM:
					return this.DecodeChksum();

				case DECODE_BLOCKS:
					if (this.isLastBlock)
					{
						if (this.noHeader)
						{
							this.mode = FINISHED;
							return false;
						}
						else
						{
							this.input.SkipToByteBoundary();
							this.neededBits = 32;
							this.mode = DECODE_CHKSUM;
							return true;
						}
					}

					var type = this.input.PeekBits(3);
					if (type < 0)
					{
						return false;
					}
					this.input.DropBits(3);

					if ((type & 1) != 0)
					{
						this.isLastBlock = true;
					}
					switch (type >> 1)
					{
						case DeflaterConstants.STORED_BLOCK:
							this.input.SkipToByteBoundary();
							this.mode = DECODE_STORED_LEN1;
							break;
						case DeflaterConstants.STATIC_TREES:
							this.litlenTree = InflaterHuffmanTree.defLitLenTree;
							this.distTree = InflaterHuffmanTree.defDistTree;
							this.mode = DECODE_HUFFMAN;
							break;
						case DeflaterConstants.DYN_TREES:
							this.dynHeader = new InflaterDynHeader();
							this.mode = DECODE_DYN_HEADER;
							break;
						default:
							throw new SharpZipBaseException("Unknown block type " + type);
					}
					return true;

				case DECODE_STORED_LEN1:
				{
					if ((this.uncomprLen = this.input.PeekBits(16)) < 0)
					{
						return false;
					}
					this.input.DropBits(16);
					this.mode = DECODE_STORED_LEN2;
				}
				goto case DECODE_STORED_LEN2; /* fall through */

				case DECODE_STORED_LEN2:
				{
					var nlen = this.input.PeekBits(16);
					if (nlen < 0)
					{
						return false;
					}
					this.input.DropBits(16);
					if (nlen != (this.uncomprLen ^ 0xffff))
					{
						throw new SharpZipBaseException("broken uncompressed block");
					}
					this.mode = DECODE_STORED;
				}
				goto case DECODE_STORED;/* fall through */

				case DECODE_STORED:
				{
					var more = this.outputWindow.CopyStored(this.input, this.uncomprLen);
					this.uncomprLen -= more;
					if (this.uncomprLen == 0)
					{
						this.mode = DECODE_BLOCKS;
						return true;
					}
					return !this.input.IsNeedingInput;
				}

				case DECODE_DYN_HEADER:
					if (!this.dynHeader.Decode(this.input))
					{
						return false;
					}

					this.litlenTree = this.dynHeader.BuildLitLenTree();
					this.distTree = this.dynHeader.BuildDistTree();
					this.mode = DECODE_HUFFMAN;
					goto case DECODE_HUFFMAN; /* fall through */

				case DECODE_HUFFMAN:
				case DECODE_HUFFMAN_LENBITS:
				case DECODE_HUFFMAN_DIST:
				case DECODE_HUFFMAN_DISTBITS:
					return this.DecodeHuffman();

				case FINISHED:
					return false;

				default:
					throw new SharpZipBaseException("Inflater.Decode unknown mode");
			}
		}

		/// <summary>
		/// Sets the preset dictionary.  This should only be called, if
		/// needsDictionary() returns true and it should set the same
		/// dictionary, that was used for deflating.  The getAdler()
		/// function returns the checksum of the dictionary needed.
		/// </summary>
		/// <param name="buffer">
		/// The dictionary.
		/// </param>
		public void SetDictionary(byte[] buffer)
		{
			this.SetDictionary(buffer, 0, buffer.Length);
		}

		/// <summary>
		/// Sets the preset dictionary.  This should only be called, if
		/// needsDictionary() returns true and it should set the same
		/// dictionary, that was used for deflating.  The getAdler()
		/// function returns the checksum of the dictionary needed.
		/// </summary>
		/// <param name="buffer">
		/// The dictionary.
		/// </param>
		/// <param name="offset">
		/// The offset into buffer where the dictionary starts.
		/// </param>
		/// <param name="len">
		/// The length of the dictionary.
		/// </param>
		/// <exception cref="InvalidOperationException">
		/// No dictionary is needed.
		/// </exception>
		/// <exception cref="SharpZipBaseException">
		/// The adler checksum for the buffer is invalid
		/// </exception>
		public void SetDictionary(byte[] buffer, int offset, int len)
		{
			if (!this.IsNeedingDictionary)
			{
				throw new InvalidOperationException();
			}

			this.adler.Update(buffer, offset, len);
			if ((int)this.adler.Value != this.readAdler)
			{
				throw new SharpZipBaseException("Wrong adler checksum");
			}
			this.adler.Reset();
			this.outputWindow.CopyDict(buffer, offset, len);
			this.mode = DECODE_BLOCKS;
		}

		/// <summary>
		/// Sets the input.  This should only be called, if needsInput()
		/// returns true.
		/// </summary>
		/// <param name="buf">
		/// the input.
		/// </param>
		public void SetInput(byte[] buf)
		{
			this.SetInput(buf, 0, buf.Length);
		}

		/// <summary>
		/// Sets the input.  This should only be called, if needsInput()
		/// returns true.
		/// </summary>
		/// <param name="buffer">
		/// The source of input data
		/// </param>
		/// <param name="offset">
		/// The offset into buffer where the input starts.
		/// </param>
		/// <param name="length">
		/// The number of bytes of input to use.
		/// </param>
		/// <exception cref="InvalidOperationException">
		/// No input is needed.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// The off and/or len are wrong.
		/// </exception>
		public void SetInput(byte[] buffer, int offset, int length)
		{
			this.input.SetInput(buffer, offset, length);
			this.totalIn += length;
		}

		/// <summary>
		/// Inflates the compressed stream to the output buffer.  If this
		/// returns 0, you should check, whether needsDictionary(),
		/// needsInput() or finished() returns true, to determine why no
		/// further output is produced.
		/// </summary>
		/// <param name = "buf">
		/// the output buffer.
		/// </param>
		/// <returns>
		/// the number of bytes written to the buffer, 0 if no further
		/// output can be produced.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// if buf has length 0.
		/// </exception>
		/// <exception cref="FormatException">
		/// if deflated stream is invalid.
		/// </exception>
		public int Inflate(byte[] buf)
		{
			return this.Inflate(buf, 0, buf.Length);
		}

		/// <summary>
		/// Inflates the compressed stream to the output buffer.  If this
		/// returns 0, you should check, whether needsDictionary(),
		/// needsInput() or finished() returns true, to determine why no
		/// further output is produced.
		/// </summary>
		/// <param name = "buf">
		/// the output buffer.
		/// </param>
		/// <param name = "offset">
		/// the offset into buffer where the output should start.
		/// </param>
		/// <param name = "len">
		/// the maximum length of the output.
		/// </param>
		/// <returns>
		/// the number of bytes written to the buffer, 0 if no further output can be produced.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// if len is &lt;= 0.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// if the offset and/or len are wrong.
		/// </exception>
		/// <exception cref="FormatException">
		/// if deflated stream is invalid.
		/// </exception>
		public int Inflate(byte[] buf, int offset, int len)
		{
			if (len < 0)
			{
				throw new ArgumentOutOfRangeException("len < 0");
			}

			// Special case: len may be zero
			if (len == 0)
			{
				if (this.IsFinished == false)
				{ // -jr- 08-Nov-2003 INFLATE_BUG fix..
					this.Decode();
				}
				return 0;
			}
			/*
						// Check for correct buff, off, len triple
						if (off < 0 || off + len >= buf.Length) {
							throw new ArgumentException("off/len outside buf bounds");
						}
			*/
			var count = 0;
			int more;
			do
			{
				if (this.mode != DECODE_CHKSUM)
				{
					/* Don't give away any output, if we are waiting for the
					* checksum in the input stream.
					*
					* With this trick we have always:
					*   needsInput() and not finished()
					*   implies more output can be produced.
					*/
					more = this.outputWindow.CopyOutput(buf, offset, len);
					this.adler.Update(buf, offset, more);
					offset += more;
					count += more;
					this.totalOut += more;
					len -= more;
					if (len == 0)
					{
						return count;
					}
				}
			} while (this.Decode() || (this.outputWindow.GetAvailable() > 0 && this.mode != DECODE_CHKSUM));
			return count;
		}

		/// <summary>
		/// Returns true, if the input buffer is empty.
		/// You should then call setInput(). 
		/// NOTE: This method also returns true when the stream is finished.
		/// </summary>
		public bool IsNeedingInput
		{
			get
			{
				return this.input.IsNeedingInput;
			}
		}

		/// <summary>
		/// Returns true, if a preset dictionary is needed to inflate the input.
		/// </summary>
		public bool IsNeedingDictionary
		{
			get
			{
				return this.mode == DECODE_DICT && this.neededBits == 0;
			}
		}

		/// <summary>
		/// Returns true, if the inflater has finished.  This means, that no
		/// input is needed and no output can be produced.
		/// </summary>
		public bool IsFinished
		{
			get
			{
				return this.mode == FINISHED && this.outputWindow.GetAvailable() == 0;
			}
		}

		/// <summary>
		/// Gets the adler checksum.  This is either the checksum of all
		/// uncompressed bytes returned by inflate(), or if needsDictionary()
		/// returns true (and thus no output was yet produced) this is the
		/// adler checksum of the expected dictionary.
		/// </summary>
		/// <returns>
		/// the adler checksum.
		/// </returns>
		public int Adler
		{
			get
			{
				return this.IsNeedingDictionary ? this.readAdler : (int)this.adler.Value;
			}
		}

		/// <summary>
		/// Gets the total number of output bytes returned by inflate().
		/// </summary>
		/// <returns>
		/// the total number of output bytes.
		/// </returns>
		public int TotalOut
		{
			get
			{
				return this.totalOut;
			}
		}

		/// <summary>
		/// Gets the total number of processed compressed input bytes.
		/// </summary>
		/// <returns>
		/// The total number of bytes of processed input bytes.
		/// </returns>
		public int TotalIn
		{
			get
			{
				return this.totalIn - this.RemainingInput;
			}
		}

#if TEST_HAK
		/// <summary>
		/// -jr test hak trying to figure out a bug
		///</summary>
		public int UnseenInput {
			get {
				return totalIn - ((input.AvailableBits + 7) >> 3);
			}
		}
		
		/// <summary>
		/// -jr test hak trying to figure out a bug
		///</summary>
		public int PlainTotalIn {
			get {
				return totalIn;
			}
		}
#endif

		/// <summary>
		/// Gets the number of unprocessed input bytes.  Useful, if the end of the
		/// stream is reached and you want to further process the bytes after
		/// the deflate stream.
		/// </summary>
		/// <returns>
		/// The number of bytes of the input which have not been processed.
		/// </returns>
		public int RemainingInput
		{
			get
			{
				return this.input.AvailableBytes;
			}
		}
	}
}
