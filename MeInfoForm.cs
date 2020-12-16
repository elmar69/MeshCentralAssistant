﻿using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace MeshAssistant
{
    public partial class MeInfoForm : Form
    {
        private MainForm parent;

        public MeInfoForm(MainForm parent)
        {
            this.parent = parent;
            InitializeComponent();
            stateListView.Items.Add(new ListViewItem("Loading..."));
            versionsListView.Items.Add(new ListViewItem("Loading..."));
            parent.agent.RequestIntelAmtState();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MeInfoForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            parent.meInfoForm = null;
        }

        public void updateInfo(Dictionary<string, object> state)
        {
            stateListView.Items.Clear();
            versionsListView.Items.Clear();
            if (state.ContainsKey("Versions"))
            {
                Dictionary<string, object> versions = (Dictionary<string, object>)state["Versions"];
                foreach (string k in versions.Keys)
                {
                    versionsListView.Items.Add(new ListViewItem(new string[] { k, versions[k].ToString() }));
                }
            }

            int flags = -1;
            int ProvisioningMode = -1;
            int ProvisioningState = -1;
            if (state.ContainsKey("Flags")) { flags = (int)state["Flags"]; }
            if (state.ContainsKey("ProvisioningMode")) { ProvisioningMode = (int)state["ProvisioningMode"]; }
            if (state.ContainsKey("ProvisioningState")) { ProvisioningState = (int)state["ProvisioningState"]; }
            string stateStr = "Unknown";
            if (ProvisioningState == 0) { stateStr = "Not Activated (Pre)"; }
            else if (ProvisioningState == 1) { stateStr = "Not Activated (In)"; }
            else if (ProvisioningState == 2) {
                stateStr = "Activated";
                if (flags >= 0) {
                    if ((flags & 2) != 0) { stateStr += ", CCM"; }
                    if ((flags & 4) != 0) { stateStr += ", ACM"; }
                }
            }
            stateListView.Items.Add(new ListViewItem(new string[] { "State", stateStr }));

            if (state.ContainsKey("UUID")) { stateListView.Items.Add(new ListViewItem(new string[] { "UUID", state["UUID"].ToString() })); }

            if (state.ContainsKey("net0"))
            {
                Dictionary<string, object> net = (Dictionary<string, object>)state["net0"];
                if (net.ContainsKey("enabled"))
                {
                    int enabled = (int)net["enabled"];
                    stateListView.Items.Add(new ListViewItem(new string[] { "net0", ((enabled != 0)?"Enabled":"Disabled") }));
                }
                if (net.ContainsKey("dhcpEnabled"))
                {
                    int dhcpEnabled = (int)net["dhcpEnabled"];
                    string x = ((dhcpEnabled != 0) ? "Enabled" : "Disabled");
                    if (net.ContainsKey("dhcpMode")) { x += ", " + (string)net["dhcpMode"]; }
                    stateListView.Items.Add(new ListViewItem(new string[] { "  DHCP", x }));
                }
                if (net.ContainsKey("mac"))
                {
                    string mac = (string)net["mac"];
                    stateListView.Items.Add(new ListViewItem(new string[] { "  MAC", mac }));
                }
                if (net.ContainsKey("address"))
                {
                    string address = (string)net["address"];
                    stateListView.Items.Add(new ListViewItem(new string[] { "  IP", address }));
                }
            }

        }
    }
}