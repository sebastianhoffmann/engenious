using System;
using System.Collections.Generic;

namespace engenious.Content.Pipeline
{
    public delegate void BuildMessageDel(object sender, BuildMessageEventArgs e);

    public interface IContentContext : IDisposable
    {
        List<string> Dependencies { get; }

        void RaiseBuildMessage(string filename, string message, BuildMessageEventArgs.BuildMessageType messageType);

        event BuildMessageDel BuildMessage;
    }
}