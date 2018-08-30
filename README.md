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
| Public Key | PubKey | The public key generated in the Imburse Portal | `example-public-key`
| Public Key | PriKey | The public key generated in the Imburse Portal | `example-private-key`
| Random Nonce | Nonce | Any random alpha-numeric string. Prevents replay attacks. `Note that Imburse will block duplice requests with thats had a nonce come through already` | `example-private-key`
| Epoch time-stamp | epoch | Seconds since epoch. See [Calculating Epoch](#calculating-epoch)  | `example-private-key`

###### Calculating Epoch

Below shows how to get the total seconds since epoch. Its important to **note** that Imburse works to a UTC time stamp. If you accidently have local time, the authentication will not pass as we dont accept requests that are older than 5 minutes.

```sh
(Date.UtcNow - DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
```

