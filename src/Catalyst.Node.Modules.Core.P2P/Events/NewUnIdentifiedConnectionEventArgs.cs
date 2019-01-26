using System;
using Catalyst.Helpers.IO;
using Catalyst.Helpers.Logger;

namespace Catalyst.Node.Modules.Core.P2P.Events
{
    public class NewUnIdentifiedConnectionEventArgs : EventArgs
    {
        /// <summary>
        /// </summary>
        /// <param name="connection"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public NewUnIdentifiedConnectionEventArgs(Connection connection)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            Log.Message("NewUnIdentifiedConnectionEventArgs");
            Connection = connection;
        }

        internal Connection Connection { get; set; }
    }
}