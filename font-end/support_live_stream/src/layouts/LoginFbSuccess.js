import { Col, Result, Row } from 'antd';
import { SmileOutlined } from '@ant-design/icons';
import React, { useEffect } from 'react';
import queryString from 'query-string';

const LoginFbSuccess = () => {
    useEffect(() => {
        const param = queryString.parseUrl(window.location.href).query;
        localStorage.setItem("isReady", param.isSuccess || false);
    }, []);

    return (
        <Row
            style={{ height: '100%' }}
        >
            <Col span={24}
                style={{ margin: "auto" }}
            >
                <Result
                    icon={<SmileOutlined />}
                    title="Kết nối Facebook thành công."
                />
            </Col>
        </Row>
    );
};

export default LoginFbSuccess;