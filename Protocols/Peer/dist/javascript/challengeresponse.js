/**
 * @fileoverview
 * @enhanceable
 * @suppress {messageConventions} JS Compiler reports an error if a variable or
 *     field starts with 'MSG_' and isn't a translatable message.
 * @public
 */
// GENERATED CODE -- DO NOT EDIT!

goog.provide('proto.ADL.Protocol.Peer.ChallengeResponse');

goog.require('jspb.BinaryReader');
goog.require('jspb.BinaryWriter');
goog.require('jspb.Message');


/**
 * Generated by JsPbCodeGenerator.
 * @param {Array=} opt_data Optional initial data array, typically from a
 * server response, or constructed directly in Javascript. The array is used
 * in place and becomes part of the constructed object. It is not cloned.
 * If no data is provided, the constructed object will be empty, but still
 * valid.
 * @extends {jspb.Message}
 * @constructor
 */
proto.ADL.Protocol.Peer.ChallengeResponse = function(opt_data) {
  jspb.Message.initialize(this, opt_data, 0, -1, null, null);
};
goog.inherits(proto.ADL.Protocol.Peer.ChallengeResponse, jspb.Message);
if (goog.DEBUG && !COMPILED) {
  proto.ADL.Protocol.Peer.ChallengeResponse.displayName = 'proto.ADL.Protocol.Peer.ChallengeResponse';
}


if (jspb.Message.GENERATE_TO_OBJECT) {
/**
 * Creates an object representation of this proto suitable for use in Soy templates.
 * Field names that are reserved in JavaScript and will be renamed to pb_name.
 * To access a reserved field use, foo.pb_<name>, eg, foo.pb_default.
 * For the list of reserved names please see:
 *     com.google.apps.jspb.JsClassTemplate.JS_RESERVED_WORDS.
 * @param {boolean=} opt_includeInstance Whether to include the JSPB instance
 *     for transitional soy proto support: http://goto/soy-param-migration
 * @return {!Object}
 */
proto.ADL.Protocol.Peer.ChallengeResponse.prototype.toObject = function(opt_includeInstance) {
  return proto.ADL.Protocol.Peer.ChallengeResponse.toObject(opt_includeInstance, this);
};


/**
 * Static version of the {@see toObject} method.
 * @param {boolean|undefined} includeInstance Whether to include the JSPB
 *     instance for transitional soy proto support:
 *     http://goto/soy-param-migration
 * @param {!proto.ADL.Protocol.Peer.ChallengeResponse} msg The msg instance to transform.
 * @return {!Object}
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.ADL.Protocol.Peer.ChallengeResponse.toObject = function(includeInstance, msg) {
  var f, obj = {
    type: jspb.Message.getFieldWithDefault(msg, 1, 0),
    signednonce: jspb.Message.getFieldWithDefault(msg, 2, ""),
    publickey: jspb.Message.getFieldWithDefault(msg, 3, "")
  };

  if (includeInstance) {
    obj.$jspbMessageInstance = msg;
  }
  return obj;
};
}


/**
 * Deserializes binary data (in protobuf wire format).
 * @param {jspb.ByteSource} bytes The bytes to deserialize.
 * @return {!proto.ADL.Protocol.Peer.ChallengeResponse}
 */
proto.ADL.Protocol.Peer.ChallengeResponse.deserializeBinary = function(bytes) {
  var reader = new jspb.BinaryReader(bytes);
  var msg = new proto.ADL.Protocol.Peer.ChallengeResponse;
  return proto.ADL.Protocol.Peer.ChallengeResponse.deserializeBinaryFromReader(msg, reader);
};


/**
 * Deserializes binary data (in protobuf wire format) from the
 * given reader into the given message object.
 * @param {!proto.ADL.Protocol.Peer.ChallengeResponse} msg The message object to deserialize into.
 * @param {!jspb.BinaryReader} reader The BinaryReader to use.
 * @return {!proto.ADL.Protocol.Peer.ChallengeResponse}
 */
proto.ADL.Protocol.Peer.ChallengeResponse.deserializeBinaryFromReader = function(msg, reader) {
  while (reader.nextField()) {
    if (reader.isEndGroup()) {
      break;
    }
    var field = reader.getFieldNumber();
    switch (field) {
    case 1:
      var value = /** @type {number} */ (reader.readInt32());
      msg.setType(value);
      break;
    case 2:
      var value = /** @type {string} */ (reader.readString());
      msg.setSignednonce(value);
      break;
    case 3:
      var value = /** @type {string} */ (reader.readString());
      msg.setPublickey(value);
      break;
    default:
      reader.skipField();
      break;
    }
  }
  return msg;
};


/**
 * Serializes the message to binary data (in protobuf wire format).
 * @return {!Uint8Array}
 */
proto.ADL.Protocol.Peer.ChallengeResponse.prototype.serializeBinary = function() {
  var writer = new jspb.BinaryWriter();
  proto.ADL.Protocol.Peer.ChallengeResponse.serializeBinaryToWriter(this, writer);
  return writer.getResultBuffer();
};


/**
 * Serializes the given message to binary data (in protobuf wire
 * format), writing to the given BinaryWriter.
 * @param {!proto.ADL.Protocol.Peer.ChallengeResponse} message
 * @param {!jspb.BinaryWriter} writer
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.ADL.Protocol.Peer.ChallengeResponse.serializeBinaryToWriter = function(message, writer) {
  var f = undefined;
  f = message.getType();
  if (f !== 0) {
    writer.writeInt32(
      1,
      f
    );
  }
  f = message.getSignednonce();
  if (f.length > 0) {
    writer.writeString(
      2,
      f
    );
  }
  f = message.getPublickey();
  if (f.length > 0) {
    writer.writeString(
      3,
      f
    );
  }
};


/**
 * optional int32 type = 1;
 * @return {number}
 */
proto.ADL.Protocol.Peer.ChallengeResponse.prototype.getType = function() {
  return /** @type {number} */ (jspb.Message.getFieldWithDefault(this, 1, 0));
};


/** @param {number} value */
proto.ADL.Protocol.Peer.ChallengeResponse.prototype.setType = function(value) {
  jspb.Message.setProto3IntField(this, 1, value);
};


/**
 * optional string signedNonce = 2;
 * @return {string}
 */
proto.ADL.Protocol.Peer.ChallengeResponse.prototype.getSignednonce = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 2, ""));
};


/** @param {string} value */
proto.ADL.Protocol.Peer.ChallengeResponse.prototype.setSignednonce = function(value) {
  jspb.Message.setProto3StringField(this, 2, value);
};


/**
 * optional string publicKey = 3;
 * @return {string}
 */
proto.ADL.Protocol.Peer.ChallengeResponse.prototype.getPublickey = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 3, ""));
};


/** @param {string} value */
proto.ADL.Protocol.Peer.ChallengeResponse.prototype.setPublickey = function(value) {
  jspb.Message.setProto3StringField(this, 3, value);
};


