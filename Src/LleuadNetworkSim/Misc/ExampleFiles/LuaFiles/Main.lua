--P100

require("./Misc");
require("_Definitions/Message");

local NodePorts = { };

local HasSentDiscovery = false;

local function SetupNodePorts()
    for i = 0, (Port_GetCount() -1) do
        NodePorts[i] = { 
            key = "Unknown" 
        };
    end
    
    table.remove(NodePorts, "D");
end

local function Size(tbl)
    local count = 0
    for _ in pairs(table) do
        count = count + 1
    end
    return count;
end

local function HandleDiscovery(Msg)

    print("Handling discovery on port " .. Msg.Port .. " from " .. Msg.SenderAddress);
    
    NewMsg = Msg_GetNewMessage();
        
    NewMsg.SenderAddress = Node_ID;
    NewMsg.ResponseRequired = false;
    NewMsg.MessageType = "Discovery/Response";
    NewMsg.SetPayload(json.serialize(NodePorts));

    Msg_Send(Msg.Port, NewMsg);
end

local function HandleDiscoveryResponse(Msg)
    
    print("Handling discovery response - " .. Msg.GetPayload());

    if Msg.GetPayload() == nil then
        print("Payload was null")
        
        return;
    end
    
    if NodePorts[Msg.Port] ~= nil then
        NodePorts[Msg.Port].Addrs.Insert(json.deserialize(Msg.GetPayload()));
    else
        NodePorts[Msg.Port] = {
            Key = Msg.SenderAddress,
            Addrs = {}
        };
    end
    
    print(json.serialize(NodePorts));
end

---@param Msg Message
local function ProcessMessage(Msg)

    print("Processing " .. Msg.ID .. " of type " .. Msg.MessageType);

    if Msg.MessageType == "DEBUG" then
        return 12;
    elseif Msg.MessageType == "Discovery" then
        HandleDiscovery(Msg);
    elseif Msg.MessageType == "Discovery/Response" then
        HandleDiscoveryResponse(Msg);
    elseif Msg.MessageType == "DEFAULT" then
        return 1;
    else
        return math.random(1, 10);
    end
end

local function Discover()

    if Port_GetCount() == 0 then
        print("no ports to discover");
        return;
    end

    for i = 0, math.random(20, 80) do
        --wait    
    end

    for i = 0, (Port_GetCount() -1) do
        Msg = Msg_GetNewMessage();
        Msg.SenderAddress = Node_ID;
        Msg.ResponseRequired = true;
        Msg.MessageType = "Discovery"

        print("sending discovery message on port " .. i);
        
        Msg_Send(i, Msg);
    end

    HasSentDiscovery = true;
end

local function Load()
    NodePorts = Reg_Load("NodePorts") or {D = -1};
    
    HasSentDiscovery = Reg_Load("SentDiscovery") or false;
end

local function Save()
    npRes = Reg_Save("NodePorts", NodePorts);
    sdRes = Reg_Save("SentDiscovery", HasSentDiscovery);
end

function Process(_, FirstLoad)

    Load();

    if FirstLoad then
        SetupNodePorts();
        Discover();
    end

    if not HasSentDiscovery then Discover() end;
    
    if Backlog_GetCount() ~= 0 then
        Msg = Backlog_Get();
        
        print("Message received from " .. Msg.SenderAddress)

        Port = ProcessMessage(Msg);

        if Msg ~= nil and Port ~= nil then
            Msg_DirectToPort(Port, Msg.ID);
        end
    else
        --print("No messages to process");
    end
    
    Save();
    
    return nil;
end

Process();
