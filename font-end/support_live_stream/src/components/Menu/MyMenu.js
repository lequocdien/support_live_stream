import React, { useEffect, useState } from 'react';
import { Menu } from 'antd';
import { Link } from 'react-router-dom';
import {
    SettingFilled,
    FacebookOutlined,
    ClearOutlined,
    TeamOutlined
} from '@ant-design/icons';

const MyMenu = ({ path }) => {
    const [isReady, setIsReady] = useState("false");
    const [pageId, setPageId] = useState("false");

    const adminMenu = (
        <Menu
            defaultSelectedKeys={[path.params.key]}
            // defaultOpenKeys={['sub1']}
            mode="inline"
            theme="dark"
            inlineCollapsed={true}
        >
            <Menu.Item key="5" icon={<TeamOutlined />}>
                <Link to="/manageruser" >Quản lý người dùng</Link>
            </Menu.Item>
            <Menu.Item key="1" icon={<SettingFilled />}>
                <Link to="/config">Cấu hình</Link>
            </Menu.Item>
            <Menu.Item key="2" icon={<FacebookOutlined />} disabled={isReady === 'false' || pageId === null ? true : false}>
                <Link to="/livestream">Livestream</Link>
            </Menu.Item>
            {/* <Menu.Item key="3" icon={<CommentOutlined />} disabled={isReady === 'false' || pageId === null ? true : false}>
                <Link to="/converstion">Hội thoại</Link>
            </Menu.Item> */}
            <Menu.Item key="4" icon={<ClearOutlined />} disabled={isReady === 'false' || pageId === null ? true : false}>
                <Link to="/blacklist" >Sổ đen</Link>
            </Menu.Item>
            {/* <Menu.SubMenu key="sub1" icon={<BarChartOutlined />} title="Báo cáo" disabled={isReady == 'true' || pageId === null ? false : true}>
                <Menu.Item key="5">Biểu đồ thống kê</Menu.Item>
            </Menu.SubMenu> */}
        </Menu>
    )

    const userMenu = (
        <Menu
            defaultSelectedKeys={[path.params.key]}
            // defaultOpenKeys={['sub1']}
            mode="inline"
            theme="dark"
            inlineCollapsed={true}
        >
            <Menu.Item key="1" icon={<SettingFilled />}>
                <Link to="/config">Cấu hình</Link>
            </Menu.Item>
            <Menu.Item key="2" icon={<FacebookOutlined />} disabled={isReady === 'false' || pageId === null ? true : false}>
                <Link to="/livestream">Livestream</Link>
            </Menu.Item>
            {/* <Menu.Item key="3" icon={<CommentOutlined />} disabled={isReady === 'false' || pageId === null ? true : false}>
                <Link to="/converstion">Hội thoại</Link>
            </Menu.Item> */}
            <Menu.Item key="4" icon={<ClearOutlined />} disabled={isReady === 'false' || pageId === null ? true : false}>
                <Link to="/blacklist" >Sổ đen</Link>
            </Menu.Item>
            {/* <Menu.SubMenu key="sub1" icon={<BarChartOutlined />} title="Báo cáo" disabled={isReady == 'true' || pageId === null ? false : true}>
                <Menu.Item key="5">Biểu đồ thống kê</Menu.Item>
            </Menu.SubMenu> */}
        </Menu>
    )

    useEffect(() => {
        setIsReady(localStorage.getItem("isReady") || false);
        setPageId(localStorage.getItem("pageId") || null);
    })

    return (
        localStorage.getItem('role') && localStorage.getItem('role') === "ADMIN" ? adminMenu : userMenu
    );
};

export default MyMenu;