--P10

local cjson = require "cjson";

data = {
    A = 12,
    B = "B"
};

json = cjson.encode(data);

print(json);