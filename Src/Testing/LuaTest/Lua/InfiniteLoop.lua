
--local json = require "json"

i = Get_Value();

function Run()

    print("Got here")
    
    local MiniData = {}
    MiniData.NestedValue = 12

    local Test = {}
    Test.Data1 = "hello data chunk";
    Test.value = 23;
    Test.NestedVal = MiniData;

    print("Got here")

    jsonData = json.serialize({ 1, 2, 3 });
    --
    --print(jsonData);
    
    return fib(120);
end

function fib(n)
    
    i = i + 1;
    
    print("fib called " .. i .. " times.");
    
    if (n == 0 or n == 1) then
        return 1;
    else
        return fib(n - 1) + fib(n - 2);
    end
end