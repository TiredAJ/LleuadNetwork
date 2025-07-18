
MiniData = {
    NestedValue = 12
};

Test = {
    Data1 = "hello data chunk",
    value = 23,
    NestedVal = MiniData
};

function Run()
    jsonData = json.serialize(Test);

    print(jsonData);

    return nil;
end