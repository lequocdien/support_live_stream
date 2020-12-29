import { Breadcrumb, Button, Card, Col, Layout, Result, Row, Select, Space } from 'antd';
import React, { useEffect, useState } from 'react';

const Config = () => {
    const userName = localStorage.getItem("userName");
    const isReady = localStorage.getItem("isReady");
    const token = localStorage.getItem("access-token");

    const [pages, setPages] = useState([]);

    useEffect(() => {
        getListPage().then(data => {
            setPages(data);
        }).catch(err => console.log(err));
    }, []);

    const getListPage = () => {
        return new Promise((resolve, reject) => {
            fetch(`${process.env.REACT_APP_API_HOST}/page`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'access_token': token
                },
            }).then(response => {
                if (response.status !== 200) {
                    return reject(response);
                }
                return response.json();
            }).then(data => {
                resolve(data);
            }).catch((err) => {
                reject("ERROR: " + err)
            });
        });
    }

    const loginFacebook = () => {
        const w = 600;
        const h = 400;
        const left = (window.screen.width / 2) - (w / 2);
        const top = (window.screen.height / 2) - h;
        const url = `https://www.facebook.com/v9.0/dialog/oauth?client_id=${process.env.REACT_APP_CLIENT_ID}&display=popup&redirect_uri=${process.env.REACT_APP_REDIRECT_URL}&state={'username': '${userName}'}&response_type=code&scope=pages_messaging, pages_show_list, pages_manage_engagement, pages_manage_metadata, pages_read_user_content, pages_manage_ads, pages_manage_posts, pages_read_engagement, publish_to_groups, publish_video`;

        const winLogin = window.open(url, "title", 'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=no, copyhistory=no, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
    }

    return (
        <Layout style={{ height: "100%" }} >
            <Row>
                <Col>
                    <Breadcrumb className="App-content-header">
                        <Breadcrumb.Item>Cấu hình</Breadcrumb.Item>
                    </Breadcrumb>
                </Col>
            </Row>

            <Row>
                <Space direction="vertical" style={{ width: "100%" }}>
                    <Card
                        title="Kết nối đến Facebook"
                        type="inner"
                    >
                        {
                            isReady == "true" ? (
                                <Result
                                    style={{ width: "100%", background: "#fff" }}
                                    status="success"
                                    title="Bạn đã liên kết với Facebook."
                                    subTitle="Thông tin của bạn sẽ không bị chia sẻ qua bên thứ 3."
                                />
                            ) : (
                                    <Result
                                        style={{ width: "100%", background: "#fff" }}
                                        status="info"
                                        title="Cho phép ứng dụng truy cập đến Facebook của bạn."
                                        subTitle="Thông tin của bạn sẽ không bị chia sẻ qua bên thứ 3."
                                        extra={[
                                            <Button key={1} onClick={loginFacebook}>Đăng nhập Facebook</Button>
                                        ]}
                                    />
                                )
                        }
                    </Card>

                    <Card
                        title="Chọn Page sẽ thao tác"
                        type="inner"
                    >
                        <Row justify="center">
                            <Col xs={24} md={12}>
                                <Select style={{ width: "100%" }} defaultValue={localStorage.getItem("pageId") || null} onChange={(value) => {
                                    localStorage.setItem("pageId", value);
                                    window.location.reload();
                                }}>
                                    {
                                        pages.map((item, index) => {
                                            return <Select.Option key={item.pageId} value={item.pageId}>{item.pageId}-{item.pageName}</Select.Option>
                                        })
                                    }
                                </Select>
                            </Col>
                        </Row>
                    </Card>

                </Space>
            </Row>

            {/* <Row>
                <Result
                    style={{ width: "100%", background: "#fff" }}
                    status="info"
                    title="Successfully Purchased Cloud Server ECS!"
                    subTitle="Order number: 2017182818828182881 Cloud server configuration takes 1-5 minutes, please wait."
                    extra={[
                        <FacebookLogin
                            appId="840201613381836"
                            // autoLoad={true}
                            fields="name,email,picture"
                            scope="pages_messaging, pages_show_list, pages_manage_engagement, pages_manage_metadata, pages_read_user_content, pages_manage_ads, pages_manage_posts, pages_read_engagement, publish_to_groups"
                            callback={responseFacebook}
                            cssClass="ant-btn ant-btn-primary"
                        />,
                        <Button key="buy">Gỡ kết nối</Button>
                    ]}
                />
            </Row> */}

        </Layout>
    );
};

export default Config;