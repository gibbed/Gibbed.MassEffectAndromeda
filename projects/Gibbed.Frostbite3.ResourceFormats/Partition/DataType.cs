/* Copyright (c) 2017 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

namespace Gibbed.Frostbite3.ResourceFormats.Partition
{
    public enum DataType : byte
    {
        Void = 0,
        DbObject = 1,
        Value = 2,
        Class = 3,
        List = 4,
        Array = 5,
        String = 6,
        String2 = 7,
        Enum = 8,
        File = 9,
        Boolean = 10,
        Int8 = 11,
        UInt8 = 12,
        Int16 = 13,
        UInt16 = 14,
        Int32 = 15,
        UInt32 = 16,
        Int64 = 17,
        UInt64 = 18,
        Float32 = 19,
        Float64 = 20,
        Guid = 21,
        SHA1 = 22,
    }
}
