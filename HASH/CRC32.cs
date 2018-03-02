


// Tamir Khason http://khason.net/
//
// Released under MS-PL : 6-Apr-09
//This license governs use of the accompanying software. If you use the software, you
//accept this license. If you do not accept the license, do not use the software.

//1. Definitions
//The terms "reproduce," "reproduction," "derivative works," and "distribution" have the
//same meaning here as under U.S. copyright law.
//A "contribution" is the original software, or any additions or changes to the software.
//A "contributor" is any person that distributes its contribution under this license.
//"Licensed patents" are a contributor's patent claims that read directly on its contribution.

//2. Grant of Rights
//(A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
//(B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.

//3. Conditions and Limitations
//(A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.
//(B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, your patent license from such contributor to the software ends automatically.
//(C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution notices that are present in the software.
//(D) If you distribute any portion of the software in source code form, you may do so only under this license by including a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object code form, you may only do so under a license that complies with this license.
//(E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular purpose and non-infringement.
using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;

/// <summary>Implements a 32-bits cyclic redundancy check (CRC) hash algorithm.</summary>
/// <remarks>This class is not intended to be used for security purposes. For security applications use MD5, SHA1, SHA256, SHA384, 
/// or SHA512 in the System.Security.Cryptography namespace.</remarks>
public class CRC32
{

    #region CONSTRUCTORS
    /// <summary>Creates a CRC32 object using the <see cref="DefaultPolynomial"/>.</summary>
    public CRC32()
        : this(DefaultPolynomial) {
    }

    public int HashSizeValue { get; private set; }
    /// <summary>Creates a CRC32 object using the specified polynomial.</summary>
    /// <remarks>The polynomical should be supplied in its bit-reflected form. <see cref="DefaultPolynomial"/>.</remarks>
    [CLSCompliant(false)]
    public CRC32(uint polynomial) {
        HashSizeValue = 32;
        _crc32Table = (uint[])_crc32TablesCache[polynomial];
        if (_crc32Table == null) {
            _crc32Table = CRC32._buildCRC32Table(polynomial);
            _crc32TablesCache.Add(polynomial, _crc32Table);
        }
        Initialize();
    }

    // static constructor
    static CRC32() {
        _crc32TablesCache = Hashtable.Synchronized(new Hashtable());
        _defaultCRC = new CRC32();
    }
    #endregion

    #region PROPERTIES
    /// <summary>Gets the default polynomial (used in WinZip, Ethernet, etc.)</summary>
    /// <remarks>The default polynomial is a bit-reflected version of the standard polynomial 0x04C11DB7 used by WinZip, Ethernet, etc.</remarks>
    [CLSCompliant(false)]
    public static readonly uint DefaultPolynomial = 0xEDB88320; // Bitwise reflection of 0x04C11DB7;
    #endregion

    #region METHODS
    /// <summary>Initializes an implementation of HashAlgorithm.</summary>
    public void Initialize() {
        _crc = _allOnes;
    }

    /// <summary>Routes data written to the object into the hash algorithm for computing the hash.</summary>
    protected void HashCore(byte[] buffer, int offset, int count) {
        int limit = count + offset;
        for (int i = offset; i < limit; i++) {
            ulong ptr = (_crc & 0xFF) ^ buffer[i];
            _crc >>= 8;
            _crc ^= _crc32Table[ptr];
        }
    }

    /// <summary>Finalizes the hash computation after the last data is processed by the cryptographic stream object.</summary>
    protected byte[] HashFinal() {
        byte[] finalHash = new byte[4];
        ulong finalCRC = _crc ^ _allOnes;

        finalHash[0] = (byte)((finalCRC >> 0) & 0xFF);
        finalHash[1] = (byte)((finalCRC >> 8) & 0xFF);
        finalHash[2] = (byte)((finalCRC >> 16) & 0xFF);
        finalHash[3] = (byte)((finalCRC >> 24) & 0xFF);

        return finalHash;
    }

    /// <summary>Computes the CRC32 value for the given ASCII string using the <see cref="DefaultPolynomial"/>.</summary>
    public static int Compute(string asciiString) {
        _defaultCRC.Initialize();
        return ToInt32(_defaultCRC.ComputeHash(asciiString));
    }

    /// <summary>Computes the CRC32 value for the given input stream using the <see cref="DefaultPolynomial"/>.</summary>
    public static int Compute(Stream inputStream) {
        _defaultCRC.Initialize();
        return ToInt32(_defaultCRC.ComputeHash(inputStream));
    }

    /// <summary>Computes the CRC32 value for the input data using the <see cref="DefaultPolynomial"/>.</summary>
    public static int Compute(byte[] buffer) {
        _defaultCRC.Initialize();
        return ToInt32(_defaultCRC.ComputeHash(buffer));
    }

    /// <summary>Computes the hash value for the input data using the <see cref="DefaultPolynomial"/>.</summary>
    public static int Compute(byte[] buffer, int offset, int count) {
        _defaultCRC.Initialize();
        return ToInt32(_defaultCRC.ComputeHash(buffer, offset, count));
    }

    /// <summary>Computes the hash value for the given ASCII string.</summary>
    /// <remarks>The computation preserves the internal state between the calls, so it can be used for computation of a stream data.</remarks>
    public byte[] ComputeHash(string asciiString) {
        byte[] rawBytes = ASCIIEncoding.ASCII.GetBytes(asciiString);
        return ComputeHash(rawBytes);
    }

    /// <summary>Computes the hash value for the given input stream.</summary>
    /// <remarks>The computation preserves the internal state between the calls, so it can be used for computation of a stream data.</remarks>
    new public byte[] ComputeHash(Stream inputStream) {
        byte[] buffer = new byte[4096];
        int bytesRead;
        while ((bytesRead = inputStream.Read(buffer, 0, 4096)) > 0) {
            HashCore(buffer, 0, bytesRead);
        }
        return HashFinal();
    }

    /// <summary>Computes the hash value for the input data.</summary>
    /// <remarks>The computation preserves the internal state between the calls, so it can be used for computation of a stream data.</remarks>
    new public byte[] ComputeHash(byte[] buffer) {
        return ComputeHash(buffer, 0, buffer.Length);
    }

    /// <summary>Computes the hash value for the input data.</summary>
    /// <remarks>The computation preserves the internal state between the calls, so it can be used for computation of a stream data.</remarks>
    new public byte[] ComputeHash(byte[] buffer, int offset, int count) {
        HashCore(buffer, offset, count);
        return HashFinal();
    }
    #endregion

    #region PRIVATE SECTION
    private static uint _allOnes = 0xffffffff;
    private static CRC32 _defaultCRC;
    private static Hashtable _crc32TablesCache;
    private uint[] _crc32Table;
    private uint _crc;

    // Builds a crc32 table given a polynomial
    private static uint[] _buildCRC32Table(uint polynomial) {
        uint crc;
        uint[] table = new uint[256];

        // 256 values representing ASCII character codes. 
        for (int i = 0; i < 256; i++) {
            crc = (uint)i;
            for (int j = 8; j > 0; j--) {
                if ((crc & 1) == 1)
                    crc = (crc >> 1) ^ polynomial;
                else
                    crc >>= 1;
            }
            table[i] = crc;
        }

        return table;
    }

    private static int ToInt32(byte[] buffer) {
        return BitConverter.ToInt32(buffer, 0);
    }
    #endregion

    public static byte[] ComputeCrc32(byte[] buffer, int start, int len) {
        byte[] result = new CRC32().ComputeHash(buffer, start, len);
        return new byte[] { result[3], result[2], result[1], result[0] };
    }
}