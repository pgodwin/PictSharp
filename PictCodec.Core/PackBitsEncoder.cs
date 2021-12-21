using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictCodec
{
    /*
     * Licensed to the Apache Software Foundation (ASF) under one or more
     * contributor license agreements.  See the NOTICE file distributed with
     * this work for additional information regarding copyright ownership.
     * The ASF licenses this file to You under the Apache License, Version 2.0
     * (the "License"); you may not use this file except in compliance with
     * the License.  You may obtain a copy of the License at
     *
     *      http://www.apache.org/licenses/LICENSE-2.0
     *
     * Unless required by applicable law or agreed to in writing, software
     * distributed under the License is distributed on an "AS IS" BASIS,
     * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
     * See the License for the specific language governing permissions and
     * limitations under the License.
     */
    /// <summary>
    /// Ported from Apache Commons Image Library 
    /// Licensed under the Apache 2.0 license
    /// https://github.com/fulcrumapp/sanselan-android/blob/master/src/main/java/org/apache/sanselan/common/PackBits.java
    /// </summary>
    /// 
    public class PackBitsEncoder
    {
        private int findNextDuplicate(byte[] bytes, int start)
        {
            //		int last = -1;
            if (start >= bytes.Length)
                return -1;

            byte prev = bytes[start];

            for (int i = start + 1; i < bytes.Length; i++)
            {
                byte b = bytes[i];

                if (b == prev)
                    return i - 1;

                prev = b;
            }

            return -1;
        }

        private int findRunLength(byte[] bytes, int start)
        {
            byte b = bytes[start];

            int i;

            for (i = start + 1; (i < bytes.Length) && (bytes[i] == b); i++)
                ;

            return i - start;
        }

        public byte[] compress(byte[] bytes)
        {
            List<sbyte> baos = new List<sbyte>(bytes.Length * 2); // max length 1 extra byte for every 128

            int ptr = 0;
            int count = 0;
            while (ptr < bytes.Length)
            {
                count++;
                int dup = findNextDuplicate(bytes, ptr);

                if (dup == ptr) // write run length
                {
                    int len = findRunLength(bytes, dup);
                    int actual_len = Math.Min(len, 128);
                    baos.Add((sbyte)-(actual_len - 1));
                    baos.Add((sbyte)bytes[ptr]);
                    ptr += actual_len;
                }
                else
                { // write literals
                    int len = dup - ptr;

                    if (dup > 0)
                    {
                        int runlen = findRunLength(bytes, dup);
                        if (runlen < 3) // may want to discard next run.
                        {
                            int nextptr = ptr + len + runlen;
                            int nextdup = findNextDuplicate(bytes, nextptr);
                            if (nextdup != nextptr) // discard 2-byte run
                            {
                                dup = nextdup;
                                len = dup - ptr;
                            }
                        }
                    }

                    if (dup < 0)

                        len = bytes.Length - ptr;
                    int actual_len = Math.Min(len, 128);

                    baos.Add((sbyte)(actual_len - 1));
                    for (int i = 0; i < actual_len; i++)
                    {
                        baos.Add((sbyte)bytes[ptr]);
                        ptr++;
                    }
                }
            }
            byte[] result = (byte[])(Array)baos.ToArray();

            return result;

        }
    }
}
