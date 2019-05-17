#region LICENSE

/**
* Copyright (c) 2019 Catalyst Network
*
* This file is part of Catalyst.Node <https://github.com/catalyst-network/Catalyst.Node>
*
* Catalyst.Node is free software: you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation, either version 2 of the License, or
* (at your option) any later version.
*
* Catalyst.Node is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with Catalyst.Node. If not, see <https://www.gnu.org/licenses/>.
*/

#endregion

using System;
using System.Security;
using Catalyst.Common.Interfaces.Cli;
using Catalyst.Common.Interfaces.Cryptography;

namespace Catalyst.Common.Cryptography
{
    public class ConsolePasswordReader
        : IPasswordReader
    {
        private const int MaxLength = 255;

        private readonly IUserOutput _userOutput;
        public ConsolePasswordReader(IUserOutput userOutput) { _userOutput = userOutput; }
        
        public SecureString ReadSecurePassword(string passwordContext = "Please enter your password")
        {
            var pwd = new SecureString();
            ReadCharsFromConsole(_userOutput, passwordContext, (c, i) => pwd.AppendChar(c), i => pwd.RemoveAt(i));

            pwd.MakeReadOnly();
            return pwd;
        }

        private static int ReadCharsFromConsole(IUserOutput userOutput,
            string passwordContext,
            Action<char, int> appendChar,
            Action<int> removeChar,
            int maxLength = MaxLength)
        {
            Console.WriteLine(passwordContext);
            var waitForInput = true;
            var inputLength = 0;
            while (waitForInput)
            {
                var keyInfo = Console.ReadKey(true);
                if (keyInfo.Key != ConsoleKey.Enter)
                {
                    if (keyInfo.Key == ConsoleKey.Backspace)
                    {
                        if (inputLength == 0)
                        {
                            continue;
                        }

                        removeChar(inputLength - 1);
                        inputLength--;
                        userOutput.Write(@" ");
                    }
                    else
                    {
                        appendChar(keyInfo.KeyChar, inputLength);
                        inputLength++;
                        userOutput.Write(@"*");
                        if (inputLength != maxLength)
                        {
                            continue;
                        }

                        userOutput.WriteLine($"Max password length reached ({maxLength})");
                        waitForInput = false;
                    }
                }
                else
                {
                    userOutput.WriteLine(string.Empty);
                    waitForInput = false;
                }
            }

            return inputLength;
        }
    }
}
