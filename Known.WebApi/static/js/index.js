﻿var Index = {

    leftTree: null,
    mainTabs: null,

    show: function () {
        this.leftTree = mini.get('leftTree');
        this.mainTabs = mini.get('mainTabs');

        this.leftTree.on('nodeclick', this.treeNodeClick);
        this.mainTabs.on('activechanged', this.tabsActiveChanged);

        Toolbar.bind('tbNavbar', TbNavbar);
        Toolbar.bind('tbMainTabs', TbMainTabs);
    },

    treeNodeClick: function (e) {
        if (e.isLeaf) {
            Index.showTab(e.node);
        }
    },

    tabsActiveChanged: function (e) {
        var tree = Index.leftTree;
        tree.selectNode({ id: e.name });
    },

    showTab: function (item) {
        var tabs = this.mainTabs;
        var tab = tabs.getTab(item.id);
        if (!tab) {
            tab = tabs.addTab({
                name: item.id, title: item.text, url: item.url,
                iconCls: item.iconCls, showCloseButton: true
            });
        }
        tabs.activeTab(tab);
        tab.bodyEl = tabs.getTabBodyEl(tab);
        return tab;
    }

};

var TbNavbar = {

    devTool: function () {
        Index.showTab({
            id: 'devTool', iconCls: 'fa-puzzle-piece',
            text: '开发工具', url: 'Pages/Develop/DevelopView.html'
        });
    },

    todo: function () {
        Index.showTab({
            id: 'todo', iconCls: 'fa-tasks',
            text: '代办事项', url: 'Pages/System/TodoView.html'
        });
    },

    cache: function () {
        Ajax.getJson('/User/GetCodes', function (data) {
            Code.setData(data);
            Message.tips({ content: '刷新成功！' });
        });
    },

    info: function () {
        Dialog.show({
            title: '用户信息', iconCls: 'fa-user',
            url: 'Pages/System/UserInfoView.html',
            callback: function () {
                Ajax.getJson('/User/GetUserInfo', function (data) {
                });
                Toolbar.bind('tbFormUpdatePwd', {
                    save: function () {
                        Message.tips('保存成功！');
                    }
                });
            }
        });
    },

    logout: function () {
        Message.confirm('确定要退出系统？', function () {
            Ajax.postText('/signout', function () {
                location = location;
            });
        });
    }

};

var TbMainTabs = {

    home: function () {
        Index.showTab({ id: 'index' });
    },

    refresh: function () {
        var tabs = Index.mainTabs;
        var tab = tabs.getActiveTab();
        tabs.reloadTab(tab);
    },

    remove: function () {
        var tabs = Index.mainTabs;
        var tab = tabs.getActiveTab();
        if (tab.name !== 'index') {
            tabs.removeTab(tab);
        }
    },

    fullScreen: function () {
    }

};

$(function () {

    $('.dropdown-toggle').click(function (event) {
        $(this).parent().addClass('open');
        return false;
    });
    $(document).click(function (event) {
        $('.dropdown').removeClass('open');
    });

    mini.parse();
    Index.show();

});