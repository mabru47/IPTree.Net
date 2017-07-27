using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Tireless.Net.Blocking
{
    public class IPTree
    {
        private class TreeNode
        {
            public TreeNode NextNodeBitNotSet
            {
                get;
                set;
            }

            public TreeNode NextNodeBitSet
            {
                get;
                set;
            }

            private Boolean isBlocked;

            public Boolean IsBlocked
            {
                get
                {
                    return this.isBlocked || this.NextNodeBitNotSet == null && this.NextNodeBitSet == null;
                }
                set
                {
                    isBlocked = value;
                }
            }

            public String Identifier
            {
                get;
                set;
            }
        }

        private Dictionary<AddressFamily, TreeNode> roots;

        public Int32 NodeCount
        {
            get;
            private set;
        }

        public Int32 NetworkCount
        {
            get;
            private set;
        }

        public IPTree()
        {
            this.roots = new Dictionary<AddressFamily, TreeNode>();
        }

        public void AddNetwork(IPAddress network, Int32 mask, String identifier = null)
        {
            var addressBytes = network.GetAddressBytes();

            if (this.roots.ContainsKey(network.AddressFamily) == false)
                this.roots.Add(network.AddressFamily, new TreeNode());

            var currentNode = this.roots[network.AddressFamily];
            for (int i = 0; i < mask; i++)
            {
                var bitset = (addressBytes[i / 8] & (0x80 >> (i % 8))) > 0;

                if (bitset)
                {
                    if (currentNode.NextNodeBitSet == null)
                        currentNode.NextNodeBitSet = this.CreateTreeNode();
                    currentNode = currentNode.NextNodeBitSet;
                }
                else
                {
                    if (currentNode.NextNodeBitNotSet == null)
                        currentNode.NextNodeBitNotSet = this.CreateTreeNode();
                    currentNode = currentNode.NextNodeBitNotSet;
                }
                if (i == mask - 1)
                {
                    currentNode.IsBlocked = true;
                    currentNode.Identifier = identifier;
                    this.NetworkCount++;
                }
            }
        }

        public void AddIPAddress(IPAddress ipAddress, String identifier = null)
        {
            this.AddNetwork(ipAddress, ipAddress.GetAddressBytes().Length * 8, identifier);
        }

        private TreeNode CreateTreeNode()
        {
            this.NodeCount++;
            return new TreeNode();
        }

        public String IsBlocked(IPAddress client)
        {
            var addressBytes = client.GetAddressBytes();

            if (this.roots.ContainsKey(client.AddressFamily) == false)
                return null;

            var currentNode = this.roots[client.AddressFamily];
            int i = 0;
            for (; i < addressBytes.Length * 8; i++)
            {
                var bitset = (addressBytes[i / 8] & (0x80 >> (i % 8))) > 0;

                if (bitset)
                {
                    if (currentNode.NextNodeBitSet == null)
                        break;
                    currentNode = currentNode.NextNodeBitSet;
                }
                else
                {
                    if (currentNode.NextNodeBitNotSet == null)
                        break;
                    currentNode = currentNode.NextNodeBitNotSet;
                }
                if (currentNode.IsBlocked == true)
                {
                    return currentNode.Identifier ?? "NULL";
                }
            }
            return null;
        }
    }
}
