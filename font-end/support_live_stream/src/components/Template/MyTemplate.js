import React, { useEffect, useState } from 'react';
import { Layout, Avatar, Space, Popover } from 'antd';
import {
    MenuUnfoldOutlined,
    MenuFoldOutlined,
    UserOutlined
} from '@ant-design/icons';
import './MyTemplate.css';
import MyMenu from '../Menu/MyMenu';

const { Header, Content, Footer, Sider } = Layout;

const MyTemplate = (props) => {
    const [collapsed, setCollapsed] = useState(false);
    const [popover, setPopover] = useState(false);
    return (
        <Layout style={{ height: "100%" }} >
            <Sider className="MyTemplate-sider"
                collapsed={collapsed}
            >
                <div className="MyTemplate-logo" />
                <MyMenu path={props.path} />
            </Sider>
            <Content style={{ marginBottom: "24px" }}>
                <Layout>
                    <Header className="MyTemplate-header"
                    >
                        <Space style={{ justifyContent: "space-between", width: "100%" }} >

                            {
                                React.createElement(collapsed ? MenuUnfoldOutlined : MenuFoldOutlined, {
                                    className: 'MyTemplate-trigger',
                                    onClick: () => { setCollapsed(!collapsed) },
                                })
                            }

                            <div className="my-template__header--avatar" >
                                <div className="my-template__header--popover">
                                    <Popover
                                        content={
                                            <ul className="ant-menu ant-menu-dark ant-menu-root ant-menu-inline">
                                                <li className="ant-menu-item"><a onClick={(e) => {
                                                    e.preventDefault();
                                                    localStorage.clear();
                                                    window.location.reload();
                                                }}>Đăng xuất</a></li>
                                            </ul>
                                        }
                                        title={localStorage.getItem("userName")}
                                        trigger="click"
                                        visible={popover}
                                        onVisibleChange={() => {
                                            setPopover(!popover)
                                        }}
                                    >

                                        <Avatar size="large" icon={<UserOutlined />} />
                                    </Popover>
                                </div>
                            </div>
                        </Space>
                    </Header>
                    <Content className="MyTemplate-content"
                    >
                        {props.children}
                    </Content>
                    {/* <Footer className="MyTemplate-footer" >Footer</Footer> */}
                </Layout>
            </Content >
        </Layout >
    );
};

export default MyTemplate;