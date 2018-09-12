var CryptoJS = require("crypto-js");

var publicKey = pm.environment.get("publicKey");
var privateKey = CryptoJS.enc.Base64.parse(pm.environment.get("privateKey"));

var timestamp = (new Date).getTime() / 1000;

var nonce = timestamp;
    
var body = request.data;

if (body.length > 0){
    var utf8Body = CryptoJS.enc.Utf8.parse(body);
    var hashedBody = CryptoJS.SHA256(utf8Body);
    var bodySignature = CryptoJS.enc.Base64.stringify(hashedBody);
}
else {
    var bodySignature = "";
}

var unsignedSignature = publicKey + ":" + nonce + ":" + timestamp + ":" + bodySignature;

var utf8Signature = CryptoJS.enc.Utf8.parse(unsignedSignature);
var hashedSignature = CryptoJS.HmacSHA256(utf8Signature, privateKey);
var signedSignature = CryptoJS.enc.Base64.stringify(hashedSignature);

var hmac = publicKey + ":" + nonce + ":" + timestamp + ":" + signedSignature;

pm.variables.set("token", hmac);