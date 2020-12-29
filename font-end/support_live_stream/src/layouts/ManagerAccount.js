import React from 'react';
import { Badge, Breadcrumb, Button, Card, Col, Descriptions, Layout, Row, Table } from 'antd';

const ManagerAccount = () => {

    const FbAccColumn = [
        {
            title: "ID",
        },
        {
            title: "Name",
        },
        {
            title: "Access Token",
        }
    ]

    return (
        <Layout style={{ height: "100%" }} >
            <Row>
                <Col>
                    <Breadcrumb className="App-content-header">
                        <Breadcrumb.Item>Manager Account</Breadcrumb.Item>
                    </Breadcrumb>
                </Col>
            </Row>

            <div className="ManagerAccount-my_account">
                <Row gutter={[16, 16]}>
                    <Col>
                        <Card title="My Account">
                            <Descriptions title="User Info" layout="vertical"
                                bordered
                                column={{ xs: 1, sm: 1, md: 3 }}
                            >
                                <Descriptions.Item label="UserName">lequocdien</Descriptions.Item>
                                <Descriptions.Item label="Password">1234</Descriptions.Item>
                                <Descriptions.Item label="Phone">0359595530</Descriptions.Item>
                                <Descriptions.Item label="Gender">Nam</Descriptions.Item>
                                <Descriptions.Item label="Created Time">2018-04-24 18:00:00</Descriptions.Item>
                                <Descriptions.Item label="Status" span={3}>
                                    <Badge status="processing" text="Active" />
                                </Descriptions.Item>
                                <Descriptions.Item label="Address">
                                    Data disk type: MongoDB
                                <br />
                                Database version: 3.4
                                <br />
                                Package: dds.mongo.mid
                                <br />
                                Storage space: 10 GB
                                <br />
                                Replication factor: 3
                                <br />
                                Region: East China 1<br />
                                </Descriptions.Item>
                            </Descriptions>
                        </Card>
                    </Col>
                </Row>
            </div>

            <Row justify="center">
                <Col xs={24} lg={24}>
                    <Card title="Facebook Account">
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
                        <Button style={{marginBottom: 16}}>Add Facebook Account</Button>
                        <Table
                            // dataSource={phoneData}
                            columns={FbAccColumn}
                        // loading={isLoadingPhone}
                        />
                    </Card>
                </Col>
            </Row>
        </Layout>
    );
};

export default ManagerAccount;