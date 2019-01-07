<?php
# Generated by the protocol buffer compiler.  DO NOT EDIT!
# source: Peer.proto

namespace ADL\Protocol\Peer\PeerProtocol;

use Google\Protobuf\Internal\GPBType;
use Google\Protobuf\Internal\RepeatedField;
use Google\Protobuf\Internal\GPBUtil;

/**
 * Generated from protobuf message <code>ADL.Protocol.Peer.PeerProtocol.PeerInfoRequest</code>
 */
class PeerInfoRequest extends \Google\Protobuf\Internal\Message
{
    /**
     * Generated from protobuf field <code>string ping = 1;</code>
     */
    private $ping = '';

    /**
     * Constructor.
     *
     * @param array $data {
     *     Optional. Data for populating the Message object.
     *
     *     @type string $ping
     * }
     */
    public function __construct($data = NULL) {
        \GPBMetadata\Peer::initOnce();
        parent::__construct($data);
    }

    /**
     * Generated from protobuf field <code>string ping = 1;</code>
     * @return string
     */
    public function getPing()
    {
        return $this->ping;
    }

    /**
     * Generated from protobuf field <code>string ping = 1;</code>
     * @param string $var
     * @return $this
     */
    public function setPing($var)
    {
        GPBUtil::checkString($var, True);
        $this->ping = $var;

        return $this;
    }

}

// Adding a class alias for backwards compatibility with the previous class name.
class_alias(PeerInfoRequest::class, \ADL\Protocol\Peer\PeerProtocol_PeerInfoRequest::class);

