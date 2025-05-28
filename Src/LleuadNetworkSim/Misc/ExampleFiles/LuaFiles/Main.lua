--P100

require("./Misc");
require("_Definitions/Message");

local NodePorts = { };

local ShouldSendDiscovery = true;
local DiscoveryPacketsSent = 0;
local DiscoveryPacketsReturned = 0;

local function SetupNodePorts()
    for i = 0, (Port_GetCount() -1) do
        NodePorts[i] = { 
            Key = "Unknown",
            Addrs = { }
        };
    end
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
    
    DiscoveryPacketsReturned = DiscoveryPacketsReturned + 1; 
    
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
    
    ShouldSendDiscovery = true;
    
    print(json.serialize(NodePorts));
end

local function SearchAddress(Addr)
    for key, value in ipairs(NodePorts) do
        if key ~= nil and value.Key ~= Addr and value.Addrs ~= nil then
            for key2, value2 in ipairs(value.Addrs) do
                if key2 ~= nil and value2.Key == Addr then
                    return key;
                end
            end
        elseif value.Key == Addr then
            return key;
        end
    end
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

    print("Discovering");
    
    if Port_GetCount() == 0 then
        print("no ports to discover");
        return;
    end

    for i = 1, Port_GetCount() do
        Msg = Msg_GetNewMessage();
        Msg.SenderAddress = Node_ID;
        Msg.ResponseRequired = true;
        Msg.MessageType = "Discovery"

        print("sending discovery message on port " .. i);
        
        Msg_Send(i, Msg);
        
        DiscoveryPacketsSent = DiscoveryPacketsSent + 1; 
    end

    ShouldSendDiscovery = false;
end

local function Load()
    tempNodePorts = Reg_Load("NodePorts");

    if tempNodePorts ~= nil then
        NodePorts = tempNodePorts;    
    end
    
    ShouldSendDiscovery = Reg_Load("ShouldSendDiscovery");
    DiscoveryPacketsSent = Reg_Load("DiscoveryPacketsSent") or 0;
    DiscoveryPacketsReturned = Reg_Load("DiscoveryPacketsReturned") or 0;
end

local function IsReadyToStartProcessing()
    if DiscoveryPacketsSent > 0 and DiscoveryPacketsReturned == DiscoveryPacketsSent then
        return true;
    else
        return false;
    end
end

local function Save()
    Reg_Save("NodePorts", NodePorts);
    Reg_Save("ShouldSendDiscovery", ShouldSendDiscovery);
    Reg_Save("DiscoveryPacketsSent", DiscoveryPacketsSent);
    Reg_Save("DiscoveryPacketsReturned", DiscoveryPacketsReturned);
end

function Process(NilVal, FirstLoad)

    if FirstLoad then
        print("First load")
        SetupNodePorts();
        print("First time discovering");
        Discover();
    else
        print("Not first load")
        Load();
    end

    if ShouldSendDiscovery then
        Discover()
    else
        print("Not sending discovery");
    end;
    
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

