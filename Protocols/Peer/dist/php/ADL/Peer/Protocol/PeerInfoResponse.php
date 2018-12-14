<?php
# Generated by the protocol buffer compiler.  DO NOT EDIT!
# source: Peer.proto

namespace ADL\Peer\Protocol;

use Google\Protobuf\Internal\GPBType;
use Google\Protobuf\Internal\RepeatedField;
use Google\Protobuf\Internal\GPBUtil;

/**
 * Generated from protobuf message <code>ADL.Peer.Protocol.PeerInfoResponse</code>
 */
class PeerInfoResponse extends \Google\Protobuf\Internal\Message
{
    /**
     * Generated from protobuf field <code>string pong = 1;</code>
     */
    private $pong = '';

    /**
     * Constructor.
     *
     * @param array $data {
     *     Optional. Data for populating the Message object.
     *
     *     @type string $pong
     * }
     */
    public function __construct($data = NULL) {
        \GPBMetadata\Peer::initOnce();
        parent::__construct($data);
    }

    /**
     * Generated from protobuf field <code>string pong = 1;</code>
     * @return string
     */
    public function getPong()
    {
        return $this->pong;
    }

    /**
     * Generated from protobuf field <code>string pong = 1;</code>
     * @param string $var
     * @return $this
     */
    public function setPong($var)
    {
        GPBUtil::checkString($var, True);
        $this->pong = $var;

        return $this;
    }

}

