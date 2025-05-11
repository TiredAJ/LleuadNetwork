
function Direct (Msg)
     if Msg.Address == "Ya Mum" then
         print("Ya Mum");
         print(Msg.Data);
     else
         print("Not Ya Mum");
         print(Msg.Data);
         
         Reg_Save(Msg.Address, Msg);
     end
end