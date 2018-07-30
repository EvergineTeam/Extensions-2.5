// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using Lidgren.Network;
using System.Collections.Generic;
using System.Linq;
using WaveEngine.Networking.Connection.Messages;
using WaveEngine.Networking.Rooms;
#endregion

namespace WaveEngine.Networking.Messages
{
    /// <summary>
    /// Helpers to write and read message buffers
    /// </summary>
    internal static class MessageBufferHelpers
    {
        #region Public Methods

        /// <summary>
        /// Reads a <see cref="ClientIncomingMessageTypes"/> type from an incoming message
        /// </summary>
        /// <param name="incomingMessage">The message</param>
        /// <returns>A <see cref="ClientIncomingMessageTypes"/> type</returns>
        internal static ClientIncomingMessageTypes ReadClientIncomingMessageType(this IncomingMessage incomingMessage)
        {
            return (ClientIncomingMessageTypes)incomingMessage.ReadByte();
        }

        /// <summary>
        /// Writes a <see cref="ClientIncomingMessageTypes"/> type to an outgoing message
        /// </summary>
        /// <param name="outgoingMessage">The message</param>
        /// <param name="messageType">The <see cref="ClientIncomingMessageTypes"/> type to write</param>
        internal static void Write(this OutgoingMessage outgoingMessage, ClientIncomingMessageTypes messageType)
        {
            outgoingMessage.Write((byte)messageType);
        }

        /// <summary>
        /// Reads a <see cref="ServerIncomingMessageTypes"/> type from an incoming message
        /// </summary>
        /// <param name="incomingMessage">The message</param>
        /// <returns>A <see cref="ServerIncomingMessageTypes"/> type</returns>
        internal static ServerIncomingMessageTypes ReadServerIncomingMessageType(this IncomingMessage incomingMessage)
        {
            return (ServerIncomingMessageTypes)incomingMessage.ReadByte();
        }

        /// <summary>
        /// Writes a <see cref="ServerIncomingMessageTypes"/> type to an outgoing message
        /// </summary>
        /// <param name="outgoingMessage">The message</param>
        /// <param name="messageType">The <see cref="ServerIncomingMessageTypes"/> type to write</param>
        internal static void Write(this OutgoingMessage outgoingMessage, ServerIncomingMessageTypes messageType)
        {
            outgoingMessage.Write((byte)messageType);
        }

        /// <summary>
        /// Reads a <see cref="EnterRoomResultCodes"/> type from an incoming message
        /// </summary>
        /// <param name="incomingMessage">The message</param>
        /// <returns>A <see cref="EnterRoomResultCodes"/> type</returns>
        internal static EnterRoomResultCodes ReadEnterRoomResultCode(this IncomingMessage incomingMessage)
        {
            return (EnterRoomResultCodes)incomingMessage.ReadByte();
        }

        /// <summary>
        /// Writes a <see cref="EnterRoomResultCodes"/> type to an outgoing message
        /// </summary>
        /// <param name="outgoingMessage">The message</param>
        /// <param name="messageType">The <see cref="EnterRoomResultCodes"/> type to write</param>
        internal static void Write(this OutgoingMessage outgoingMessage, EnterRoomResultCodes messageType)
        {
            outgoingMessage.Write((byte)messageType);
        }

        /// <summary>
        /// Reads an array of strings from an incoming message
        /// </summary>
        /// <param name="incomingMessage">The message</param>
        /// <returns>An array of strings</returns>
        internal static string[] ReadStringArray(this IncomingMessage incomingMessage)
        {
            var lenght = incomingMessage.ReadInt32();

            var array = new string[lenght];
            for (int i = 0; i < lenght; i++)
            {
                array[i] = incomingMessage.ReadString();
            }

            return array;
        }

        /// <summary>
        /// Writes an array of integers to an outgoing message
        /// </summary>
        /// <param name="outgoingMessage">The message</param>
        /// <param name="enumerable">The enumerable to write</param>
        internal static void Write(this OutgoingMessage outgoingMessage, IEnumerable<int> enumerable)
        {
            if (enumerable == null)
            {
                outgoingMessage.Write(0);
            }
            else
            {
                outgoingMessage.Write(enumerable.Count());

                foreach (var integer in enumerable)
                {
                    outgoingMessage.Write(integer);
                }
            }
        }

        /// <summary>
        /// Reads an array of integers from an incoming message
        /// </summary>
        /// <param name="incomingMessage">The message</param>
        /// <returns>An array of integers</returns>
        internal static int[] ReadInt32Array(this IncomingMessage incomingMessage)
        {
            var lenght = incomingMessage.ReadInt32();

            var array = new int[lenght];
            for (int i = 0; i < lenght; i++)
            {
                array[i] = incomingMessage.ReadInt32();
            }

            return array;
        }

        /// <summary>
        /// Writes an array of strings to an outgoing message
        /// </summary>
        /// <param name="outgoingMessage">The message</param>
        /// <param name="enumerable">The enumerable to write</param>
        internal static void Write(this OutgoingMessage outgoingMessage, IEnumerable<string> enumerable)
        {
            if (enumerable == null)
            {
                outgoingMessage.Write(0);
            }
            else
            {
                outgoingMessage.Write(enumerable.Count());

                foreach (var str in enumerable)
                {
                    outgoingMessage.Write(str);
                }
            }
        }

        /// <summary>
        /// Reads an enumerable of RoomInfo objects from an incoming message
        /// </summary>
        /// <param name="incomingMessage">The message</param>
        /// <param name="roomInfoList">The list where the RoomInfo objects will be stored</param>
        internal static void ReadRoomInfoList(this IncomingMessage incomingMessage, List<RoomInfo> roomInfoList)
        {
            var lenght = incomingMessage.ReadInt32();

            for (int i = 0; i < lenght; i++)
            {
                var newObj = RoomInfo.FromMessage(incomingMessage);
                roomInfoList.Add(newObj);
            }
        }

        /// <summary>
        /// Writes an enumerable of RoomInfo objects to an outgoing message
        /// </summary>
        /// <param name="outgoingMessage">The message</param>
        /// <param name="enumerable">The enumerable to write</param>
        internal static void Write(this OutgoingMessage outgoingMessage, IEnumerable<RoomInfo> enumerable)
        {
            if (enumerable == null)
            {
                outgoingMessage.Write(0);
            }
            else
            {
                outgoingMessage.Write(enumerable.Count());
                foreach (var obj in enumerable)
                {
                    obj.WriteToMessage(outgoingMessage);
                }
            }
        }

        /// <summary>
        /// Writes an incoming message to an outgoing message respecting the current position of the incoming message
        /// </summary>
        /// <param name="outgoingMessage">The message</param>
        /// <param name="incomingMessage">The incoming message</param>
        internal static void Write(this OutgoingMessage outgoingMessage, IncomingMessage incomingMessage)
        {
            var outgoingBuffer = (NetBuffer)outgoingMessage.InnerMessage;

            var inconmingBuffer = (NetBuffer)incomingMessage.InnerMessage;
            var offset = inconmingBuffer.PositionInBytes;
            var sizeInBytes = inconmingBuffer.LengthBytes - offset;

            outgoingBuffer.EnsureBufferSize(outgoingBuffer.LengthBits + (sizeInBytes * 8));
            outgoingBuffer.Write(inconmingBuffer.Data, offset, sizeInBytes);

            // did we write excessive bits?
            int bitsInLastByte = outgoingBuffer.LengthBits % 8;
            if (bitsInLastByte != 0)
            {
                int excessBits = 8 - bitsInLastByte;
                outgoingBuffer.LengthBits -= excessBits;
            }
        }

        /// <summary>
        /// Reads a player id from the message
        /// </summary>
        /// <param name="incomingMessage">The message</param>
        /// <returns>The player id</returns>
        internal static int ReadPlayerId(this IncomingMessage incomingMessage)
        {
            return incomingMessage.ReadInt32();
        }

        #endregion
    }
}
