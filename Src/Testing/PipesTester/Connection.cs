using System.Threading.Channels;

namespace PipesTester;

using Message = Message<string>;

public class Connection
{
    public Connection(Channel<Message> _Input, Channel<Message> _Output) {
        Input = _Input.Reader;
        Output = _Input.Writer;
    }
    
    public ChannelReader<Message> Input;
    public ChannelWriter<Message> Output;
}
