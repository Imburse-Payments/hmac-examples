# HMAC Examples

The intention of this repo is to explain how the HMAC authentication works for Imburse API access. Before anything is done, you will need both a **Public** and **Private** key that can be generated by the Imburse Admin Portal.


# What is HMAC!

Hash-based message authentication code (HMAC) provides the server and the client each with a private key that is known only to that specific server and that specific client. The client creates a unique HMAC, or hash, per request to the server by hashing the request data  with the private keys and sending it as part of a request. What makes HMAC more secure than Message Authentication Code (MAC) is that the key and the message are hashed in separate steps.

```sh
HMAC(key, msg) = H(mod1(key) || H(mod2(key) || msg))
```

This ensures the process is not susceptible to extension attacks that add to the message and can cause elements of the key to be leaked as successive MACs are created.

Once the server receives the request and regenerates its own unique HMAC, it compares the two HMACs. If they're equal, the client is trusted and the request is executed. This process is often called a secret handshake.

`Note: Imburse uses an HMACSHA256 algorythm.`

### Calculating the HMAC

##### Things you will need

The below table outlines the things you will need to be able to calculate an HMAC Signature

| Thing | Alias | Description | Example |
| ------ | ------ | ------ | ------ |
| Public Key | `pubKey` | The public key generated in the Imburse Portal | `example-public-key`
| Public Key | `priKey` | The public key generated in the Imburse Portal | `example-private-key`
| Random Nonce | `nonce` | Any random alpha-numeric string. Prevents replay attacks. `Note that Imburse will block duplice requests with thats had a nonce come through already` | `example-private-key`
| Epoch time-stamp | `epoch` | Seconds since epoch. See [Calculating Epoch](#calculating-epoch)  | `example-private-key`
| Body Content | `body` | For Post and Put requests only. This is the http body that is being sent  | 

#### Calculating Epoch

Below shows how to get the total seconds since epoch. Its important to **note** that Imburse works to a UTC time stamp. If you accidently have local time, the authentication will not pass as we dont accept requests that are older than 5 minutes.

```sh
(Date.UtcNow - DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
```

#### Creating the signature

The following formula is used for creating the HMAC:

1. Hash the `body` using an `SHA256` hash algorythm and the convert it to a base64 string. Note this is for only requests that contain a body. 
```sh
  let hashedBody = SHA256(body).toBase64String()
```
**ATTENTION!** Most Hash algorythms will take a binary array. When converting the body (most likely JSON) be sure to make it `UTF8` encoding. For example:

```sh
  UTF8Encoder.GetBytes(body)
```

2. Create the string that will be generated into an HMAC signature. Below is the formula to generate the string:

```sh
let stringToBeSigned = $"{pubKey}:{nonce}:{epoch}:{hashedBody}"

Result:

REQUEST WITH A BODY:
"some-public-key:randomgenerateduniquenonce:1535617532:GgVc9wTFOfF7EcDHiz+U/kpf3H7A5FfZ+RfA6FZ3IFA="

REQUEST WITHOUT A BODY i.e. GET REQUEST:

"some-public-key:randomgenerateduniquenonce:1535617532:"
```



3. Similar to step 2, to generate the HMAC signature sign it using the private key and then base64 encode it


```sh
  let signedSignature = HMACSHA256(stringToBeSigned).toBase64String()

  print(signedSignature)
  ......................

  "asd3ad3a3dascwTFOfF7EcDHiz+U/kpf3H7A5FfZ+RfA6FZ3IFA="
```

4.  Generate th HTTP Hmac auth header.

Add the following authorised header to your request

hmac {`pubKey`}:{`nonce`}:{`epoch`}:{`signedSignature`}