

local function Direct (Msg)
    print "Direct called";

    if Msg.Address == "Ya Mum" then
        print("Ya Mum");
        print(Msg.Data);
    else
        print("Not Ya Mum");
        print(Msg.Data);
    end
end
