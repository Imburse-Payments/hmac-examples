import org.apache.commons.codec.binary.Base64
import org.apache.commons.codec.digest.HmacUtils
import org.apache.commons.codec.digest.HmacAlgorithms
import org.apache.jmeter.protocol.http.control.Header

//JMeter - public and private keys passed from the parameters.
def publicKey = args[0] 
def privateKey = args[1]

def privateKeyBytes = Base64.decodeBase64(privateKey)
def bodySignature = ""
def timestamp = (new Date()).getTime() / 1000
def nonce = timestamp
def unsignedSignature = publicKey + ":" + nonce + ":" + timestamp + ":" + bodySignature

def utf8Signature =   unsignedSignature.getBytes("UTF-8")
def hashedSignature = HmacUtils.getInitializedMac(HmacAlgorithms.HMAC_SHA_256, privateKeyBytes).doFinal(utf8Signature)
def signedSignature = new String(Base64.encodeBase64(hashedSignature))
      
def hmac = publicKey + ":" + nonce + ":" + timestamp + ":" + signedSignature

//JMeter specific
sampler.getHeaderManager().removeHeaderNamed("Authorization")
sampler.getHeaderManager().add(new Header("Authorization","Hmac " + hmac))