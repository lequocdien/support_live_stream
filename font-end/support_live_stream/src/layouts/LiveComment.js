import React, { useEffect, useState } from 'react';
import { Breadcrumb, Button, Form, Card, Col, Input, Layout, Menu, Row, Select, Transfer, Switch, Tag, Table, List, Avatar } from 'antd';
import moment from 'moment';

const LiveComment = () => {

    const LIVE_ID = "672450930312129";

    const [isLoadingComment, setIsLoadingComment] = useState(false);
    const [isLoadingPhone, setIsLoadingPhone] = useState(false);
    const [commentData, setCommentData] = useState([]);
    const [phoneData, setPhoneData] = useState([]);

    const callEventSource = () => {
        alert("Call");
        setIsLoadingComment(true);
        const sourceComment = new EventSource(`https://localhost:44302/api/comment/live-comment?liveId=${LIVE_ID}`);
        const sourcePhone = new EventSource(`https://localhost:44302/api/comment/phone?liveId=${LIVE_ID}`);

        sourceComment.onerror = (err) => {
            alert("Error");
            sourceComment.close();
        }

        sourceComment.onopen = (e) => {
            console.log(e);
        }

        sourceComment.onmessage = function (e) {
            setIsLoadingComment(false);
            setCommentData(JSON.parse(e.data));
        }

        sourcePhone.onmessage = function (e) {
            const data = JSON.parse(e.data);
            setPhoneData(data.map((item, index) => {
                return {
                    ...item,
                    key: index
                }
            }));
        }
    }

    const phoneColumn = [
        {
            key: "DocumentId",
            title: "Name",
            dataIndex: "FbName",
            render: (value, parent) => {
                return <a href={`https://fb.com/${parent.FbId}`}>{value}</a>
            },
            width: 350
        },
        {
            key: "DocumentId",
            title: "Phone",
            dataIndex: "Phone",
            render: (value) => {
                return <Tag color="green">{value}</Tag>
            },
            width: 200
        },
        {
            key: "DocumentId",
            title: "Message",
            dataIndex: "Message",
        }
    ]

    return (
        <Layout style={{ height: "100%" }} >
            <Row>
                <Col>
                    <Breadcrumb className="App-content-header">
                        <Breadcrumb.Item>Comment</Breadcrumb.Item>
                    </Breadcrumb>
                </Col>
            </Row>

            <Row justify="space-between" gutter={[16, 16]}>
                <Col xs={24} lg={12}>
                    <div className="LiveComment-card">
                        <Card title="Account/Link Live Stream">
                            <Form
                                labelCol={{ md: 24, lg: 24 }}
                                labelAlign="left"
                                style={{ height: "250px" }}
                            >
                                <Form.Item
                                    label="Facebook"
                                    name="txtFacebookId"
                                >
                                    <Select defaultValue={1111} style={{ width: "100%" }}>
                                        <Select.Option value={1111}>fb/lequocdien39</Select.Option>
                                        <Select.Option value={1112}>fb/lequocdien39</Select.Option>
                                        <Select.Option value={1113}>fb/lequocdien39</Select.Option>
                                    </Select>
                                </Form.Item>

                                <Form.Item
                                    label="Link Live Stream"
                                    name="txtLinkStream"
                                >
                                    <Input />
                                </Form.Item>

                                <Form.Item>
                                    <Button
                                        type="primary"
                                        htmlType="submit"
                                        style={{ margin: "auto", display: "flex" }}
                                        onClick={callEventSource}
                                    >Go!</Button>
                                </Form.Item>

                            </Form>
                        </Card>
                    </div>
                </Col>

                <Col xs={24} lg={12}>
                    <div className="LiveComment-list">
                        <div className="LiveComment-card">
                            <Card title="Live Comment">
                                <List dataSource={commentData}
                                    itemLayout="horizontal"
                                    style={{ height: "250px", overflow: "auto" }}
                                    loading={isLoadingComment}
                                    renderItem={item => (
                                        <List.Item key={item.id}>
                                            <List.Item.Meta
                                                avatar={
                                                    <Avatar />
                                                }
                                                title={<a href={`https://fb.com/${item.from.id}`}>{item.from.name}</a>}
                                                description={item.message}
                                            />
                                            <div style={{ marginLeft: "5px" }} >{moment(item.created_time).format("hh:mm:ss")}</div>
                                        </List.Item>
                                    )}
                                />
                            </Card>
                        </div>
                    </div>
                </Col>
            </Row>

            <Row justify="center">
                <Col xs={24} lg={24}>
                    <Card title="Filterd Comment">
                        {/* <List dataSource={fakeData}
                            itemLayout="horizontal"
                            renderItem={item => (
                                <List.Item key={item.id}>
                                    <List.Item.Meta
                                        avatar={
                                            <Avatar src="https://zos.alipayobjects.com/rmsportal/ODTLcjxAfvqbxHnVXCYX.png" />
                                        }
                                        title={<a href="https://ant.design">{item.name.last}</a>}
                                        description={item.email}
                                    />
                                    <div>{item.CreatedTime}</div>
                                </List.Item>
                            )}
                        /> */}
                        <Table
                            dataSource={phoneData}
                            columns={phoneColumn}
                            loading={isLoadingPhone}
                        />
                    </Card>
                </Col>
            </Row>
        </Layout>
    );
};

export default LiveComment;