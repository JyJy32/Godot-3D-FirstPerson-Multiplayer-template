using Godot;
using Godot.Collections;

public partial class PlayerInfo : Node
{
    public long peerID;
    public string peerInfo;
    public bool isServer;

    public PlayerInfo(int peerID, string peerInfo, bool isServer)
    {
        this.peerID = peerID;
        this.peerInfo = peerInfo;
        this.isServer = isServer;
    }

    public Dictionary encode()
    {
        Dictionary data = new Dictionary();
        data.Add("peerID", peerID);
        data.Add("peerInfo", peerInfo);
        data.Add("isServer", isServer);
        return data;
    }

    public void decode(Dictionary data)
    {
        peerID = (long)data["peerID"];
        peerInfo = (string)data["peerInfo"];
        isServer = (bool)data["isServer"];
    }
}
